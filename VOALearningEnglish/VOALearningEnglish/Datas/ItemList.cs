using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOALearningEnglish.Datas
{
        public enum ItemType
        {
            New = 0,
            Video = 1,
            Live = 2,
        }
        class ItemList
        {
            public ItemType Type { get; set; }
            public string TextItem { get; set; }
            public string UrlLink { get; set; }
            public string ImageSource { get; set; }
            public string TextToolTip { get; set; }
    }
}
