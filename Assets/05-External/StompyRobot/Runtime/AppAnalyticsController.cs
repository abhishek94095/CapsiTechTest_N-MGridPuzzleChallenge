using System;
using System.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
    public static class AppAnalyticsController
    {
        private static Task initializationTask;

        public static void Initialize()
        {
            initializationTask = SendAppOpenEventAsync();
        }

        private static async Task SendAppOpenEventAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();
                string userId = AnalyticsService.Instance.GetAnalyticsUserID();
                if (string.IsNullOrWhiteSpace(userId)) return;

                UnityServices.ExternalUserId = userId;
                AnalyticsService.Instance.RecordEvent(new AppOpenEvent(
                    UnityEngine.Application.identifier,
                    UnityEngine.Application.productName,
                    UnityEngine.Application.companyName,
                    userId));
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Unity Analytics appOpen recording failed: {exception.Message}");
            }
        }
    }

