namespace Core.Const
{
    public static class NotificationTemplateSymbol
    {
        public static readonly Dictionary<string,string> MessageContents =  new Dictionary<string, string>() 
        {
            { "FULLNAME", "<<FULLNAME>>"},
            { "FIRSTNAME", "<<FIRSTNAME>>"},
            { "LASTNAME", "<<LASTNAME>>" },
            { "PHONE", "<<PHONE>>" },
            { "CILINIC_NAME", "<<CILINIC_NAME>>" },
            { "DOCTOR_NAME", "<<DOCTOR_NAME>>" },
            { "RESULTS", "<<RESULTS>>" },
            { "DOCTORADVICE", "<<DOCTORADVICE>>" },
            { "EXAMINATION_FILE", "<<EXAMINATION_FILE>>" },
            { "APPOINTMENT_DATE", "<<APPOINTMENT_DATE>>" },
            { "APPOINTMENT_TIME", "<<APPOINTMENT_TIME>>" },
            { "PATIENT_NAME", "<<PATIENT_NAME>>" },
            { "PATIENT_CODE", "<<PATIENT_CODE>>" },
            { "USER_EMAIL", "<<USER_EMAIL>>" },
            { "RE_EXAMINATION", "<<RE_EXAMINATION>>" }
        };
    }

 

    public static class NotificationForApp
    {
        public static readonly Dictionary<string, string> NotificationTitle = new Dictionary<string, string>()
        {
            { "Task",        "Công việc"},
            { "Project",     "Dự án"},
            { "Document",    "Văn bản"},
            { "WorkSchedule", "Lịch làm việc"},
            { "MeetingContent", "Cuộc họp"},
            { "Appoinment", "Lịch hẹn"},
        };

        public static readonly Dictionary<string, string> NotificationContent = new Dictionary<string, string>()
        {
            { "TaskAssign",     "Có công việc mới - Id task : <<Id>>"},
            { "TaskUpdate",     "Có cập nhật công việc - Id công việc <<Id>>" },
            { "ProjectAssign",  "Có dự án mới - Id dự án : <<Id>>"},
            { "ProjectUpdate",  "Có cập nhật dự án - Id dự án : <<Id>>"},
            { "ReceiveDocument","Nhận văn bản mới - Id văn bản <<Id>>"},
            { "WorkSchedule",   "Có lịch làm việc mới - Id văn bản <<Id>>" },
            { "MeetingContent", "Có nội dung họp mới - Id nội dung họp <<Id>>"},
            { "AppoinmentNew",  "[Api.Appoinment.New]"},
            { "AppoinmentUpdate", "[Api.Appoinment.Update]"},
        };
    }
}