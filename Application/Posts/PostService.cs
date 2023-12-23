using Application.Helpers;
using Contract;
using Contract.Common.Excels;
using Contract.Posts;
using Core.Const;
using Core.Exceptions;
using Domain.Posts;
using Domain.StaticFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using SqlServ4r.Repository.Files;
using SqlServ4r.Repository.PostCategories;
using SqlServ4r.Repository.Posts;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Volo.Abp.DependencyInjection;

namespace Application.Posts
{
    public class PostService : ServiceBase, IPostService, ITransientDependency
    {
        private readonly PostRepository _postRepository;
        private readonly PostCategoryRepository _postCategoryRepository;
        private readonly StaticFileRepository _staticFileRepository;

        public PostService(PostRepository postRepository, PostCategoryRepository postCategoryRepository, StaticFileRepository staticFileRepository)
        {
            _postRepository = postRepository;
            _postCategoryRepository = postCategoryRepository;
            _staticFileRepository = staticFileRepository;
        }

        public async Task<ApiResponseBase<PostDto>> CreateAsync(CreateUpdatePostDto input)
        {
            ApiResponseBase<PostDto> result = new ApiResponseBase<PostDto>();
            try
            {
                var post = ObjectMapper.Map<Post>(input);
                await _postRepository.AddAsync(post);
                result.Data = ObjectMapper.Map<PostDto>(post);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ApiResponseBase<PostDto>> DeleteAsync(Guid id)
        {
            ApiResponseBase<PostDto> result = new ApiResponseBase<PostDto>();
            try
            {
                var post = await _postRepository.FirstOrDefaultAsync(x => x.Id == id);

                if (post == null)
                    throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);

                post.IsDeleted = true;
                await _postRepository.UpdateAsync(post);
                result.Data = ObjectMapper.Map<Post, PostDto>(post);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ApiResponseBase<List<PostDto>>> GetListAsync()
        {
            ApiResponseBase<List<PostDto>> result = new ApiResponseBase<List<PostDto>>();
            try
            {
                var posts = await _postRepository.GetQueryable().Where(x => x.IsDeleted == false).Include(x => x.Picture).Include(x => x.PostCategory).ToListAsync();

                result.Data = ObjectMapper.Map<List<Post>, List<PostDto>>(posts);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;

        }
        public async Task<ApiResponseBase<PostDto>> GetByIdAsync(Guid id)
        {
            ApiResponseBase<PostDto> result = new ApiResponseBase<PostDto>();
            try
            {
                var posts = await _postRepository.GetQueryable()
                .Where(x => x.IsDeleted == false
                    && x.Id == id)
                .Include(x => x.Picture)
                .Include(x => x.PostCategory).FirstOrDefaultAsync();
                result.Data = ObjectMapper.Map<Post, PostDto>(posts);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }


        public async Task<ApiResponseBase<List<PostDto>>> GetListPagingAsync(BaseFilterPagingDto filter)
        {
            ApiResponseBase<List<PostDto>> result = new ApiResponseBase<List<PostDto>>();
            try
            {
                var posts = _postRepository.GetQueryable()
                 .Where(x => x.IsDeleted == false
                     && (string.IsNullOrEmpty(filter.FilterText) ? 1 == 1 : (!string.IsNullOrEmpty(filter.FilterText) && x.Title.Trim().ToLower().Contains(filter.FilterText.Trim().ToLower())))
                 )
                 .Include(x => x.Picture)
                 .Include(x => x.PostCategory)
                 .Skip(filter.Skip)
                 .Take(filter.Take);

                result.Data = ObjectMapper.Map<List<Post>, List<PostDto>>(await posts.ToListAsync());
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;

        }

        public async Task<ApiResponseBase<PostSearchResultDto>> GetListPagingAsync(PostFitlerPagingDto filter)
        {
            ApiResponseBase<PostSearchResultDto> result = new ApiResponseBase<PostSearchResultDto>();
            try
            {
                List<Guid> postCategories = new List<Guid>();
                if (!string.IsNullOrEmpty(filter.PostCategoryIds))
                    postCategories = filter.PostCategoryIds.Split(',').Select(x => Guid.TryParse(x, out Guid guid) ? guid : Guid.Empty).ToList();

                result.Data = new PostSearchResultDto();

                var posts = _postRepository.GetQueryable()
                 .Include(x => x.Picture)
                 .Include(x => x.PostCategory)
                 .Include(x => x.CreatedUser)
                 .Where(x => x.IsDeleted == false
                        && (string.IsNullOrEmpty(filter.FilterText) ? 1 == 1 : (x.Title.Trim().ToLower().Contains(filter.FilterText.Trim().ToLower())))
                        && (string.IsNullOrEmpty(filter.PostCategorySlug) ? 1 == 1 : (x.PostCategory != null && x.PostCategory.Slug.Trim().ToLower().Equals(filter.PostCategorySlug.Trim().ToLower())))
                        && (string.IsNullOrEmpty(filter.PostSlug) ? 1 == 1 : (!string.IsNullOrEmpty(x.Slug) && x.Slug.Trim().ToLower().Equals(filter.PostSlug.Trim().ToLower())))
                        && (filter.PostCategoryId == null || filter.PostCategoryId == Guid.Empty ? 1 == 1 : x.PostCategoryId == filter.PostCategoryId)
                        && (filter.IgnorePostId == null || filter.IgnorePostId == Guid.Empty ? 1 == 1 : x.Id != filter.IgnorePostId)
                        && (filter.IsHighlight == null ? 1 == 1 : x.IsHighLight == filter.IsHighlight)
                        && (filter.IsCategoryHighlight == null ? 1 == 1 : x.PostCategory.IsHighLight == filter.IsCategoryHighlight)
                        && (postCategories.Any() ? postCategories.Any(s => s == x.PostCategoryId) : true)
                 )
                 .AsQueryable();

                if (filter.IsTopViews == true)
                    posts = posts.OrderByDescending(x => x.Views).ThenBy(x => x.ODX).ThenByDescending(x => x.CreatedDate).AsQueryable();
                else
                    posts = posts.OrderBy(x => x.ODX).OrderByDescending(x => x.CreatedDate).AsQueryable();

                List<Post> postResult;

                List<string> tags = filter.Tags != null && !string.IsNullOrEmpty(filter.Tags) ? filter.Tags.Split(',').ToList() : new List<string>();

                if (tags.Any())
                {
                    var postsData = posts.Where(x => !string.IsNullOrEmpty(x.Tags)).AsEnumerable<Post>();
                    postResult = postsData.Where(x => x.Tags!.Split(',').Select(x => x.Trim()).Intersect(tags).Count() > 0).ToList();

                    result.Data.TotalItems = postResult.Count();

                    if (filter.Take > 0)
                        postResult = postResult.Skip(filter.Skip).Take(filter.Take).ToList();
                }
                else
                {
                    result.Data.TotalItems = posts.Count();

                    if (filter.Take > 0)
                        posts = posts.Skip(filter.Skip).Take(filter.Take);

                    postResult = posts.ToList();
                }


                result.Data.Items = ObjectMapper.Map<List<Post>, List<PostDto>>(postResult);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;

        }

        public Task<ApiResponseBase<List<string>>> GetListTags(int? skip, int? take)
        {
            ApiResponseBase<List<string>> result = new ApiResponseBase<List<string>>();
            try
            {
                if (!take.HasValue || take == 0)
                    take = int.MaxValue;

                if (!skip.HasValue)
                    skip = 0;

                List<string> resultData = new List<string>();
                result.Data = resultData;

                var posts = _postRepository.GetQueryable()
                 .Where(x => x.IsDeleted == false && !string.IsNullOrEmpty(x.Tags))
                 .Select(x => x.Tags).JoinAsString(",");

                result.Data = posts?.Split(',')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .GroupBy(x => x.Trim())
                    .Select(g => new { Key = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Select(x => x.Key)
                    .Skip(skip ?? 0)
                    .Take(take ?? int.MaxValue)
                    .ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Task.FromResult(result);
        }

        public async Task<ApiResponseBase<List<PostDto>>> GetRelatedListAsync(Guid id, int take = 10)
        {
            ApiResponseBase<List<PostDto>> result = new ApiResponseBase<List<PostDto>>();
            try
            {
                var curPost = await _postRepository.FirstOrDefaultAsync(x => x.Id == id);

                if (curPost == null)
                {
                    result.Data = new List<PostDto>();

                    return result;
                }

                var posts = _postRepository.GetQueryable()
                 .Include(x => x.Picture)
                 .Include(x => x.PostCategory)
                 .Where(x => x.IsDeleted == false && (!string.IsNullOrEmpty(x.Tags) || x.PostCategoryId == curPost.PostCategoryId) && x.Id != id)
                 .OrderBy(x => x.ODX)
                 .OrderByDescending(x => x.CreatedDate)
                 .AsEnumerable<Post>();

                List<string> tags = curPost.Tags != null && !string.IsNullOrEmpty(curPost.Tags) ? curPost.Tags.Split(',').ToList() : new List<string>();

                var postResult = posts.Where(x => (!string.IsNullOrEmpty(x.Tags) && x.Tags.Split(",", StringSplitOptions.None).Select(x => x.Trim()).Intersect(tags).Count() > 0 || x.PostCategoryId == curPost.PostCategoryId)).Take(take > 0 ? take : int.MaxValue).ToList();

                result.Data = ObjectMapper.Map<List<Post>, List<PostDto>>(postResult);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ApiResponseBase<PostDto>> UpdateAsync(CreateUpdatePostDto input, Guid id)
        {
            ApiResponseBase<PostDto> result = new ApiResponseBase<PostDto>();
            try
            {
                var post = await _postRepository.FirstOrDefaultAsync(x => x.Id == id);

                if (post == null)
                    throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);

                var entity = ObjectMapper.Map(input, post);

                await _postRepository.UpdateAsync(entity);
                result.Data = ObjectMapper.Map<PostDto>(entity);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;


        }

        public async Task<ApiResponseBase<byte[]>> ExportExcel(PostFitlerPagingDto filter)
        {
            ApiResponseBase<byte[]> result = new ApiResponseBase<byte[]>();
            try
            {
                var posts = _postRepository.GetQueryable()
                 .Where(x => x.IsDeleted == false
                     && (string.IsNullOrEmpty(filter.FilterText) ? 1 == 1 : (!string.IsNullOrEmpty(filter.FilterText) && x.Title.Trim().ToLower().Contains(filter.FilterText.Trim().ToLower())))
                     && (filter.PostCategoryId == null || filter.PostCategoryId == Guid.Empty ? 1 == 1 : x.PostCategoryId == filter.PostCategoryId)
                     && (filter.IsHighlight == null ? 1 == 1 : x.IsHighLight == filter.IsHighlight)
                     && (filter.IsCategoryHighlight == null ? 1 == 1 : x.PostCategory.IsHighLight == filter.IsCategoryHighlight)
                 )
                .Include(x => x.Picture)
                .OrderBy(x => x.ODX)
                .AsQueryable();

                if (filter.Take > 0)
                    posts = posts.Skip(filter.Skip).Take(filter.Take);

                var postResult = await posts.Select(x => new
                {
                    Id = x.Id,
                    Title = x.Title,
                    PictureId = x.Picture != null ? x.Picture.Name : string.Empty,
                    Slug = x.Slug,
                    categoryId = x.PostCategoryId,
                    ShortContent = x.SortDescription,
                    Content = x.Description,
                    IsHighLight = x.IsHighLight,
                    IsVisibled = x.IsVisibled,
                    ODX = x.ODX,
                    Tags = x.Tags,
                    SEOTitle = x.SeoTitle,
                    SEOKeyword = x.SeoKeyword,
                    SEODescription = x.SeoDescription,
                }).ToListAsync();

                List<string> tags = filter.Tags != null && !string.IsNullOrEmpty(filter.Tags) ? filter.Tags.Split(',').ToList() : new List<string>();

                if (tags.Any())
                    postResult = postResult.Where(x => !string.IsNullOrEmpty(x.Tags) && x.Tags.Split(',').Intersect(tags).Count() > 0).ToList();

                // Generate Excel content
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using ExcelPackage pack = new ExcelPackage();
                ExcelWorksheet ws = pack.Workbook.Worksheets.Add(DateTime.Now.ToString("yyyy-MM-dd"));
                ws.Cells["A1"].LoadFromCollection(postResult, true, TableStyles.Light1);

                result.Data = pack.GetAsByteArray();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
        public async Task<ApiResponseBase<DataValidatorExcel>> CreatePostFromExcelFileAsync(IFormFile file)
        {
            ApiResponseBase<DataValidatorExcel> result = new ApiResponseBase<DataValidatorExcel>();

            try
            {
                if (string.IsNullOrEmpty(file.FileName))
                {
                    throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        if (worksheet.Dimension is null)
                        {
                            throw new GlobalException(HttpMessage.EmptyContent, HttpStatusCode.BadRequest);
                        }

                        int rowCount = worksheet.Dimension.End.Row;

                        var postRows = new List<PostExcelDto>();
                        DataValidatorExcel postValidator = new DataValidatorExcel();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var item = ReadUserRowFromExcel(row, worksheet);
                            if (!IsNullExcelRow(item)) break;
                            postRows.Add(item);
                            postValidator.InvalidCells.AddRange(ValidateUserRow(item));
                        }

                        if (postValidator.InvalidCells.Count < 1)
                        {
                            var urls = postRows.Where(x => !string.IsNullOrEmpty(x.PictureId)).Select(x => x.PictureId ?? string.Empty).Distinct();
                            var images = await _staticFileRepository.GetQueryable().Where(x => urls.Any(s => s == x.Name)).ToListAsync();

                            var imageNotExists = urls.Except(images.Select(x => x.URL)).Where(x => !string.IsNullOrEmpty(x)).ToList();
                            if (imageNotExists != null && imageNotExists.Any())
                            {
                                var imageEntities = imageNotExists.Select(imageUrl =>
                                {
                                    var url = new Uri(imageUrl);

                                    return new StaticFile()
                                    {
                                        Id = Guid.NewGuid(),
                                        Extension = System.IO.Path.GetExtension(url.LocalPath),
                                        Name = System.IO.Path.GetFileName(url.LocalPath),
                                        Path = url.LocalPath,
                                        FileType = Core.Enum.FileTypes.Image,
                                        URL = imageUrl
                                    };
                                });
                                await _staticFileRepository.AddRangeAsync(imageEntities);

                                var newImages = await _staticFileRepository.GetQueryable().Where(x => imageNotExists.Any(s => s == x.URL)).ToListAsync();
                                images.AddRange(newImages);
                            }

                            List<Post> posts = new List<Post>();
                            foreach (var post in postRows)
                            {
                                posts.Add(new Post()
                                {
                                    Title = post.Title ?? string.Empty,
                                    PostCategoryId = Guid.TryParse(post.PostCategoryId, out Guid postCategoryIdValue) ? postCategoryIdValue : Guid.Empty,
                                    Tags = post.Tags,
                                    SeoTitle = post.SeoTitle,
                                    Slug = URL_REWRITE(post.Title),
                                    SeoKeyword = post.SeoKeyword,
                                    SeoDescription = post.SeoDescription,
                                    SortDescription = post.SortDescription,
                                    Description = post.Description,
                                    ODX = int.TryParse(post.ODX, out int oDXValue) ? oDXValue : 0,
                                    IsHighLight = bool.TryParse(post.IsHighLight, out bool isHighLightValue) ? isHighLightValue : false,
                                    IsVisibled = bool.TryParse(post.IsVisibled, out bool isVisibleValue) ? isVisibleValue : true,
                                    PictureId = images.FirstOrDefault(x => x.Name == post.PictureId)?.Id ?? images.FirstOrDefault(x => x.URL == post.PictureId)?.Id ?? null,
                                    CreatedBy = Guid.TryParse(post.UserId, out Guid userIdValue) ? userIdValue : null,
                                    CreatedDate = post.CreateDate.IsNullOrWhiteSpace() ? null : DateTime.ParseExact(post.CreateDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)
                                    // CreatedDate = DateTime.TryParseExact(post.CreateDate, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime createDated) ? createDated : DateTime.Now
                                });
                            }

                            await _postRepository.AddRangeAsync(posts);
                        }

                        result.Data = postValidator;
                        result.Message = string.Join('\n', postValidator.InvalidCells.Select(x => $"Cell ({x.Row}, {x.Col}) is invalid"));
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
        public string URL_REWRITE(string URL)
        {
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    URL = URL.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);


            }
            URL = URL.ToLower();
            URL = Regex.Replace(URL, " ", "-");
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = URL.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public async Task<ApiResponseBase<int>> IncreaseViewsTotalAsync(Guid id)
        {
            ApiResponseBase<int> result = new ApiResponseBase<int>();

            try
            {
                var post = await _postRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == id);

                if (post != null)
                {
                    post.Views++;

                    await _postRepository.UpdateAsync(post);

                    result.Data = post.Views;
                }
                result.Message = "Post could not be found!";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }


        #region Private Method
        private static readonly string[] VietNamChar = new string[]
        {
            "aAeEoOuUiIdDyY",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ"
        };

        private PostExcelDto ReadUserRowFromExcel(int row, ExcelWorksheet worksheet)
        {
            var user = new PostExcelDto()
            {
                Id = worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                Title = worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                PictureId = worksheet.Cells[row, 3].Value?.ToString()?.Trim(),
                Slug = worksheet.Cells[row, 4].Value?.ToString()?.Trim(),
                PostCategoryId = worksheet.Cells[row, 5].Value?.ToString()?.Trim(),
                SortDescription = worksheet.Cells[row, 6].Value?.ToString()?.Trim(),
                Description = worksheet.Cells[row, 7].Value?.ToString()?.Trim(),
                IsHighLight = worksheet.Cells[row, 8].Value?.ToString()?.Trim(),
                IsVisibled = worksheet.Cells[row, 9].Value?.ToString()?.Trim(),
                ODX = worksheet.Cells[row, 10].Value?.ToString()?.Trim(),
                Tags = worksheet.Cells[row, 11].Value?.ToString()?.Trim(),
                SeoTitle = worksheet.Cells[row, 12].Value?.ToString()?.Trim(),
                SeoKeyword = worksheet.Cells[row, 13].Value?.ToString()?.Trim(),
                SeoDescription = worksheet.Cells[row, 14].Value?.ToString()?.Trim(),
                CreateDate = worksheet.Columns.Count() >= 15 ? worksheet.Cells[row, 15].Value?.ToString()?.Trim() : string.Empty,
                UserId = worksheet.Columns.Count() >= 16 ? worksheet.Cells[row, 16].Value?.ToString()?.Trim() : string.Empty,
                Row = row
            };

            return user;
        }

        private bool IsNullExcelRow(PostExcelDto post)
        {
            if (post.Title.IsNullOrWhiteSpace() & post.PostCategoryId.IsNullOrWhiteSpace())
                return false;

            return true;
        }
        private List<Cell> ValidateUserRow(PostExcelDto postExcelDto)
        {
            PostValidator validator = new PostValidator();

            var invalidCells = new List<Cell>();

            if (string.IsNullOrEmpty(postExcelDto.Title))
            {
                invalidCells.Add(new Cell()
                {
                    Row = postExcelDto.Row,
                    Col = 2,
                });
            }

            if (!string.IsNullOrEmpty(postExcelDto.Slug) && !validator.ValidateSlug(postExcelDto.Slug))
            {
                invalidCells.Add(new Cell()
                {
                    Row = postExcelDto.Row,
                    Col = 4,
                });
            }

            if (!validator.ValidateGuid(postExcelDto.PostCategoryId ?? string.Empty))
            {
                invalidCells.Add(new Cell()
                {
                    Row = postExcelDto.Row,
                    Col = 5,
                });
            }

            if (!string.IsNullOrEmpty(postExcelDto.IsHighLight) && !validator.ValidateBool(postExcelDto.IsHighLight ?? string.Empty))
            {
                invalidCells.Add(new Cell()
                {
                    Row = postExcelDto.Row,
                    Col = 8,
                });
            }

            if (!string.IsNullOrEmpty(postExcelDto.IsVisibled) && !validator.ValidateBool(postExcelDto.IsVisibled ?? string.Empty))
            {
                invalidCells.Add(new Cell()
                {
                    Row = postExcelDto.Row,
                    Col = 9,
                });
            }

            return invalidCells;
        }
        #endregion
    }
}
