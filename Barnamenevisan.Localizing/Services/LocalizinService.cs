using Barnamenevisan.Localizing.Attributes;
using Barnamenevisan.Localizing.Entity;
using Barnamenevisan.Localizing.Repository;
using Barnamenevisan.Localizing.Shared;
using Barnamenevisan.Localizing.Statics;
using Barnamenevisan.Localizing.ViewModels;

namespace Barnamenevisan.Localizing.Services
{
    public class LocalizinService : ILocalizingService
    {
        #region Fields

        private readonly ILocalizedPropertyRepository _repository;

        #endregion

        #region Constructor

        public LocalizinService(ILocalizedPropertyRepository repository)
        {
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task TranslateModelAsync<TLocalizedModel>(TLocalizedModel model)
            where TLocalizedModel : class
        {
            if (LocalizingTools.DefaultCultureName == Thread.CurrentThread.CurrentCulture.Name) return;

            var propertiesOfModelToTranslate = typeof(TLocalizedModel)
                .GetProperties()
                .Where(property => property.CustomAttributes?.Any(attribute => attribute.AttributeType == typeof(TranslateAttribute)) ?? false)
                .ToList();

            if (!propertiesOfModelToTranslate.Any()) return;

            var entityNames = propertiesOfModelToTranslate.Select(property =>
                        property.CustomAttributes
                        .First(attribute => attribute.AttributeType == typeof(TranslateAttribute))
                        .NamedArguments
                        .Where(argument => argument.MemberName == nameof(TranslateAttribute.EntityName))
                        .Select(argument => argument.TypedValue.Value?.ToString())
                        .FirstOrDefault() ?? typeof(TLocalizedModel).Name.Replace("ViewModel", "")
                ).ToList().Distinct();

            var currentCultureName = Thread.CurrentThread.CurrentCulture.Name;

            var localizedProperties = await _repository
                .GetAllAsync(lp => entityNames.Contains(lp.EntityName) && lp.CultureName == currentCultureName);

            if (localizedProperties is null || !localizedProperties.Any())
            {
                return;
            }

            foreach (var propertyToTranslate in propertiesOfModelToTranslate)
            {
                var arguments = propertyToTranslate.CustomAttributes
                            .First(attribute => attribute.AttributeType == typeof(TranslateAttribute))
                            .NamedArguments;

                var attribute = new TranslateAttribute();

                foreach (var argument in arguments)
                {
                    attribute.GetType().GetProperty(argument.MemberName).SetValue(attribute, argument.TypedValue.Value?.ToString());
                }

                if (string.IsNullOrWhiteSpace(attribute.Key))
                {
                    attribute.Key = propertyToTranslate.Name;
                }

                if (string.IsNullOrWhiteSpace(attribute.EntityName))
                {
                    attribute.EntityName = typeof(TLocalizedModel).Name.Replace("ViewModel", "");
                }

                var entityId = (int?)model.GetType().GetProperty(attribute.PropertyNameOfEntityIdInThisClass)?.GetValue(model);

                var expectedLocalizedProperty = localizedProperties
                                                    .FirstOrDefault(lp =>
                                                                    lp.Key == attribute.Key &&
                                                                    lp.EntityId.Equals(entityId) &&
                                                                    lp.CultureName == currentCultureName
                                                    );

                if (!string.IsNullOrWhiteSpace(expectedLocalizedProperty?.Value))
                {
                    propertyToTranslate.SetValue(model, expectedLocalizedProperty?.Value);
                }
            }
        }

        public async Task TranslateModelAsync<TLocalizedModel>(List<TLocalizedModel> models)
            where TLocalizedModel : class
        {
            if (models is null || !models.Any()) return;

            if (LocalizingTools.DefaultCultureName == Thread.CurrentThread.CurrentCulture.Name) return;

            var propertiesOfModelToTranslate = typeof(TLocalizedModel)
                .GetProperties()
                .Where(property => property.CustomAttributes?.Any(attribute => attribute.AttributeType == typeof(TranslateAttribute)) ?? false)
                .ToList();

            if (!propertiesOfModelToTranslate.Any()) return;

            var entityNames = propertiesOfModelToTranslate.Select(property =>
                        property.CustomAttributes
                        .First(attribute => attribute.AttributeType == typeof(TranslateAttribute))
                        .NamedArguments
                        .Where(argument => argument.MemberName == nameof(TranslateAttribute.EntityName))
                        .Select(argument => argument.TypedValue.Value?.ToString())
                        .FirstOrDefault() ?? typeof(TLocalizedModel).Name.Replace("ViewModel", "")
                ).ToList().Distinct();

            var currentCultureName = Thread.CurrentThread.CurrentCulture.Name;

            var localizedProperties = await _repository
                .GetAllAsync(lp => entityNames.Contains(lp.EntityName) && lp.CultureName == currentCultureName);

            if (localizedProperties is null || !localizedProperties.Any())
            {
                return;
            }

            foreach (var model in models)
            {
                foreach (var propertyToTranslate in propertiesOfModelToTranslate)
                {
                    var arguments = propertyToTranslate.CustomAttributes
                            .First(attribute => attribute.AttributeType == typeof(TranslateAttribute))
                            .NamedArguments;

                    var attribute = new TranslateAttribute();

                    foreach (var argument in arguments)
                    {
                        attribute.GetType().GetProperty(argument.MemberName).SetValue(attribute, argument.TypedValue.Value?.ToString());
                    }

                    if (string.IsNullOrWhiteSpace(attribute.Key))
                    {
                        attribute.Key = propertyToTranslate.Name;
                    }

                    if (string.IsNullOrWhiteSpace(attribute.EntityName))
                    {
                        attribute.EntityName = typeof(TLocalizedModel).Name.Replace("ViewModel", "");
                    }

                    var entityId = (int?)model.GetType()
                                                .GetProperty(attribute.PropertyNameOfEntityIdInThisClass)?
                                                .GetValue(model);

                    var expectedLocalizedProperty = localizedProperties
                                                        .FirstOrDefault(lp =>
                                                                        lp.Key == attribute.Key &&
                                                                        lp.EntityId.Equals(entityId) &&
                                                                        lp.CultureName == currentCultureName
                                                        );

                    if (!string.IsNullOrWhiteSpace(expectedLocalizedProperty?.Value))
                    {
                        propertyToTranslate.SetValue(model, expectedLocalizedProperty?.Value);
                    }
                }
            }
        }

        public async Task FillModelToEditByAdminAsync<TLocalizedModel>(string entityName, object entityId, LocalizableViewModel<TLocalizedModel> model)
            where TLocalizedModel : LocalizedViewModel, new()
        {
            model.LocalizedModels = new List<TLocalizedModel>();

            var localizedProperties = await _repository
                .GetAllAsync(lp => lp.EntityId == entityId && lp.EntityName == entityName);

            if (localizedProperties is null)
            {
                localizedProperties = new List<LocalizedProperty>();
            }

            var propertiesOfLocalizedModel = typeof(TLocalizedModel).GetProperties();

            foreach (var cultureName in LocalizingTools.SupportedCultures.Select(c => c.Name).Where(name => name != LocalizingTools.DefaultCultureName))
            {
                var localizedModel = new TLocalizedModel();

                foreach (var property in propertiesOfLocalizedModel)
                {
                    var expectedLocalizedProperty = localizedProperties
                        .FirstOrDefault(lp => lp.Key == property.Name && lp.CultureName == cultureName);

                    property.SetValue(localizedModel, expectedLocalizedProperty?.Value);
                }

                localizedModel.CultureName = cultureName;
                model.LocalizedModels.Add(localizedModel);
            }
        }

        public Task FillModelToEditByAdminAsync<TEntity, TLocalizedModel>(TEntity entity, LocalizableViewModel<TLocalizedModel> model)
            where TEntity : BaseEntity<int>
            where TLocalizedModel : LocalizedViewModel, new()
        {
            if (entity is null || entity?.Id <= 0) return Task.CompletedTask;

            return FillModelToEditByAdminAsync(entity.GetType().Name, entity.Id, model);
        }

        public async Task SaveLocalizations<TEntity, TKey, TLocalizedModel>(TEntity entity, LocalizableViewModel<TLocalizedModel> model)
            where TEntity : BaseEntity<TKey>
            where TLocalizedModel : LocalizedViewModel, new()
            where TKey : IEquatable<TKey>
        {
            if (entity is null) return;

            var savedLocalizedProperties = await _repository
                .GetAllAsync(lp => Convert.ChangeType(lp.EntityId, typeof(TKey)).Equals(entity.Id) && lp.EntityName == entity.GetType().Name);

            var propertiesOfLocalizedModelNeedToLocalize = typeof(TLocalizedModel)
                                            .GetProperties()
                                            .Where(property => property.Name != nameof(LocalizedViewModel.CultureName))
                                            .ToList();

            var ToInsertLocalziedProperties = new List<LocalizedProperty>();
            var ToUpdateLocalziedProperties = new List<LocalizedProperty>();

            foreach (var currentCultureName in model.LocalizedModels.Select(m => m.CultureName))
            {
                var expectedLocalizedModel = model.LocalizedModels
                    .FirstOrDefault(model => model.CultureName == currentCultureName);

                if (expectedLocalizedModel is null) continue;

                foreach (var property in propertiesOfLocalizedModelNeedToLocalize)
                {
                    var expectedLocalizedProperty = savedLocalizedProperties
                        ?.FirstOrDefault(l => l.Key == property.Name && l.CultureName == currentCultureName)
                        ?? new LocalizedProperty
                        {
                            Key = property.Name,
                            EntityId = entity.Id,
                            EntityName = entity.GetType().Name,
                            CultureName = currentCultureName
                        };

                    if (expectedLocalizedProperty.Value == (property.GetValue(expectedLocalizedModel)?.ToString() ?? ""))
                    {
                        continue;
                    }

                    expectedLocalizedProperty.Value = property.GetValue(expectedLocalizedModel)?.ToString() ?? "";

                    if (!string.IsNullOrWhiteSpace(expectedLocalizedProperty?.Value))
                    {
                        if (expectedLocalizedProperty.Id > 0)
                        {
                            ToUpdateLocalziedProperties.Add(expectedLocalizedProperty);
                        }
                        else
                        {
                            ToInsertLocalziedProperties.Add(expectedLocalizedProperty);
                        }
                    }
                }
            }

            _repository.UpdateRange(ToUpdateLocalziedProperties);
            await _repository.InsertRangeAsync(ToInsertLocalziedProperties);

            await _repository.SaveAsync();
        }

        #endregion
    }
}
