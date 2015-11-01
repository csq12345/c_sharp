using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolHelper
{
    public class SaveInfo
    {
        /// <summary>
        /// 地图类型
        /// </summary>
        public int maptype { get; set; }
        public int stopx { get; set; }
        public int stopy { get; set; }
        /// <summary>
        /// 缩放等级
        /// </summary>
        public int level { get; set; }
        public string savepath { get; set; }
        
    }
}
