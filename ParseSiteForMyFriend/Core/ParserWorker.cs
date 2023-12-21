using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseSiteForMyFriend.Core
{
    internal class ParserWorker<T> where T : class
    {
        IParser<T> parser;

        IParserSettings parserSettings;

        HtmlLoader loader;

        bool isActive;

        #region Properties

        public event Action<object, T> OnNewData;

        public event Action<object> OnCompleted;

        public IParser<T> Parser { get => parser; set => parser = value; }

        public IParserSettings Settings 
        { 
            get => parserSettings; 

            set  
            { 
                parserSettings = value;
                loader = new HtmlLoader(value);
            }

        }

        public bool IsActive { get => isActive; }

        #endregion

        public ParserWorker(IParser<T> parser)
        {
            this.parser = parser;
        }

        public ParserWorker(IParser<T> parser, IParserSettings parserSettings) : this(parser)
        {
            this.parserSettings = parserSettings;
        }

        public async void Start()
        {
            isActive = true;
            Worker();
        }

        public void Abort()
        {
            isActive &= false;
        }

        public async void Worker()
        {
            for (int i = parserSettings.StartPoint; i <= parserSettings.EndPoint; i++)
            {
                if (!isActive)
                {
                    OnCompleted?.Invoke(this);
                    return;
                }

                var source = await loader.GetSourceByPageId(i);
                var domParser = new HtmlParser();

                var document = await domParser.ParseDocumentAsync(source);

                var result = parser.Parse(document);

                OnNewData?.Invoke(this, result);
            }

            OnCompleted?.Invoke(this);
            isActive = false;
        }
    }
}
