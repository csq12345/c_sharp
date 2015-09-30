using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW_MapDown
{
   public interface IMapWorker
   {
       int StartDown(int zoomlevel, string savedir);
       /// <summary>
       /// 获取瓦片url路径
       /// </summary>
       /// <param name="x"></param>
       /// <param name="y"></param>
       /// <returns></returns>
       string GetDirectPath(int zoomlevel, int x, int y);
   }
}
