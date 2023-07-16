namespace Barnamenevisan.Localizing.ViewModels
{
    public class LocalizableViewModel<TModel> : BaseEntityViewModel<ulong>
        where TModel : LocalizedViewModel, new()
    {
        public LocalizableViewModel()
        {
            LocalizedModels = new List<TModel>() { new TModel() };
        }

        public List<TModel> LocalizedModels { get; set; }

        public List<LocalizedViewModel> PreparedLocalizedModelsForView
            => LocalizedModels.Select(x => x as LocalizedViewModel).ToList();
    }

    public class LocalizedViewModel
    {
        public string CultureName { get; set; }
    }
}
