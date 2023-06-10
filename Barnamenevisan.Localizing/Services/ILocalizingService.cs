using Barnamenevisan.Localizing.Shared;
using Barnamenevisan.Localizing.ViewModels;

namespace Barnamenevisan.Localizing.Services
{
    public interface ILocalizingService
    {
        Task FillModelToEditByAdminAsync<TLocalizedModel>(string entityName, object entityId, LocalizableViewModel<TLocalizedModel> model) where TLocalizedModel : LocalizedViewModel, new();
        Task FillModelToEditByAdminAsync<TEntity, TLocalizedModel>(TEntity entity, LocalizableViewModel<TLocalizedModel> model)
            where TEntity : BaseEntity<int>
            where TLocalizedModel : LocalizedViewModel, new();
        Task SaveLocalizations<TEntity, TKey, TLocalizedModel>(TEntity entity, LocalizableViewModel<TLocalizedModel> model)
            where TEntity : BaseEntity<TKey>
            where TKey : IEquatable<TKey>
            where TLocalizedModel : LocalizedViewModel, new();
        Task TranslateModelAsync<TLocalizedModel>(List<TLocalizedModel> models) where TLocalizedModel : class;
        Task TranslateModelAsync<TLocalizedModel>(TLocalizedModel model) where TLocalizedModel : class;
    }
}