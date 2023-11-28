using AutoMapper;

namespace Utils.Mappings
{
    public class ObjectMapper
    {

        public static modelTo ConvertObject<modelFrom,  modelTo>(modelFrom model)
        { 
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<modelFrom, modelTo>();
            });

            var mapper = mapperConfig.CreateMapper();
            var mapped = mapper.Map<modelTo>(model);

            return mapped;
        }

        public static List<modelTo> ConvertList<modelFrom, modelTo>(List<modelFrom> modelList)
        {
            var mappedList = new List<modelTo>();

            foreach ( var model in modelList) 
            {
                mappedList.Add( ConvertObject<modelFrom, modelTo>(model) );
            }

            return mappedList;
        }
    }
}
