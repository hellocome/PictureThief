using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PictureThief.Interfaces;

namespace PictureThief.Impl
{
    public class PTMyWardrobe : PictureProcessorBase
    {
        public PTMyWardrobe() :
            base("MyWardrobe")
        {

        }

        protected override bool PareparePicInfo(PageInfo pi, string content)
        {


            return true;
        }
    }
}
