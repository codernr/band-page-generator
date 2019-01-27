namespace BandPageGenerator.Services.Interfaces
{
    public interface IViewRenderer
    {
        string RenderView<TModel>(string viewName, TModel model);
    }
}
