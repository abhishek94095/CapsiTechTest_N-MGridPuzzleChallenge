using Unity.Services.Analytics;

    public sealed class AppOpenEvent : Event
    {
        public AppOpenEvent(string appId, string appName, string companyName, string userId) : base("appOpen")
        {
            AppId = appId;
            AppName = appName;
            CompanyName = companyName;
            UserId = userId;
        }

        public string AppId
        {
            set => SetParameter("appId", value);
        }

        public string AppName
        {
            set => SetParameter("appName", value);
        }

        public string CompanyName
        {
            set => SetParameter("companyName", value);
        }

        public string UserId
        {
            set => SetParameter("userId", value);
        }
    }
