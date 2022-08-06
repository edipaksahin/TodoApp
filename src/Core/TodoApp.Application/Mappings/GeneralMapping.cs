using AutoMapper;
using TodoApp.Application.Dto.Todo;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Mappings
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            CreateMap<Task, TaskDto>()
                .ReverseMap();

            CreateMap<TaskDto, Task>()
                .ReverseMap();

            CreateMap<Task, TaskViewModel>()
                .ForMember(s => s.Id, m => m.MapFrom(u => u.Id))
                .ForMember(s => s.IsCompleted, m => m.MapFrom(u => u.IsCompleted))
                .ForMember(s => s.Title, m => m.MapFrom(u => u.Title))
                .ForMember(s => s.Description, m => m.MapFrom(u => u.Description));
        }

    }
}
