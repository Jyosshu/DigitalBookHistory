using System;
using System.Collections.Generic;
using System.Text;
using DigitalBookHistoryLoader.models;

namespace DigitalBookHistoryLoader.interfaces
{
    public interface IImageRepository
    {
        List<TitleFields> GetTitleFields();
        bool CreateImageFields(List<ImageFields> imageFieldsList);
    }
}
