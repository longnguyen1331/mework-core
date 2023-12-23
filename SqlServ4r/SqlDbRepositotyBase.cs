using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq.Expressions;
using SqlServ4r.EntityFramework;

namespace SqlServ4r
{
    public interface SqlDbRepositotyBase<T> where T: class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}