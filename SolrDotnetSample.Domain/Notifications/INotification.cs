using System.Collections.Generic;

namespace SolrDotnetSample.Domain.Notifications
{
    public interface INotification
    {
        string Error { get; }
        List<string> Errors { get; }
        void AddError(string error);
        void AddError(INotification notification);
        void AddError(string error, INotification externalNotification);
        void AddErrors(IEnumerable<INotification> notifications);
        void AddErrors(IEnumerable<string> errors);
    }
}