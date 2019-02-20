using BandPageGenerator.Config;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public abstract class AbstractTemplateDataTransformer : ITemplateDataTransformer
    {
        protected readonly IDownloaderClient downloader;
        protected readonly GeneralConfig generalConfig;

        protected AbstractTemplateDataTransformer(IDownloaderClient downloader, IOptions<GeneralConfig> config)
        {
            this.downloader = downloader;
            this.generalConfig = config.Value;
        }

        public abstract Task AddTemplateDataAsync(Dictionary<string, object> templateData);

        protected async Task<IEnumerable<T>> Replace<T>(IEnumerable<T> data, Expression<Func<T, string>> memberLambda, Expression<Func<T, string>> idLambda)
        {
            var urlGetter = memberLambda.Compile();
            var idGetter = idLambda.Compile();

            var member = (MemberExpression)memberLambda.Body;
            var param = Expression.Parameter(typeof(string));
            var urlSetter = Expression.Lambda<Action<T, string>>(
                Expression.Assign(member, param), memberLambda.Parameters[0], param).Compile();

            var tasks = data.Select(async target =>
            {
                var newValue = await this.downloader.DownloadFile(
                            urlGetter(target), idGetter(target), this.generalConfig.DownloadSavePath, this.generalConfig.DownloadedBasePath);

                urlSetter(target, newValue);

                return target;
            }).ToArray();

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result);
        }
    }
}
