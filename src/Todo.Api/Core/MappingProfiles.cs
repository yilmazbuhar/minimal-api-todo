using AutoMapper;

namespace Todo.Api
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<TodoItemSaveModel,TodoItem>();
        }
    }
}
