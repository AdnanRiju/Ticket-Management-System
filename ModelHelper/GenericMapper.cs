using AutoMapper;

namespace CompanyManagement.ModelHelper
{
    public class GenericMapper<S, D>
        where S : class
        where D : class
    {
        public static D GetDestination(S sourceModelObject)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<S, D>());
            var mapper = config.CreateMapper();
            var dest = mapper.Map<D>(sourceModelObject);
            return dest;
        }
        public static List<D> GetDestinationList(List<S> sourceModelList)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<S, D>());
            var mapper = config.CreateMapper();
            var destinationList = mapper.Map<List<S>, List<D>>(sourceModelList);
            return destinationList;
        }

    }


}
