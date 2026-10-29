using MediatR;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;

namespace OrbitAOS.V6.Application.Features.Samples.Commands
{
    /// <summary>
    /// Command to create a new sample
    /// </summary>
    public class CreateSampleCommand : IRequest<SampleDto>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Handler for CreateSampleCommand
    /// </summary>
    public class CreateSampleCommandHandler : IRequestHandler<CreateSampleCommand, SampleDto>
    {
        private readonly ISampleService _sampleService;

        public CreateSampleCommandHandler(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        public async Task<SampleDto> Handle(CreateSampleCommand request, CancellationToken cancellationToken)
        {
            var dto = new CreateSampleDto
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive
            };

            return await _sampleService.CreateSampleAsync(dto);
        }
    }
}

namespace OrbitAOS.V6.Application.Features.Samples.Commands
{
    /// <summary>
    /// Command to update an existing sample
    /// </summary>
    public class UpdateSampleCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Handler for UpdateSampleCommand
    /// </summary>
    public class UpdateSampleCommandHandler : IRequestHandler<UpdateSampleCommand, Unit>
    {
        private readonly ISampleService _sampleService;

        public UpdateSampleCommandHandler(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        public async Task<Unit> Handle(UpdateSampleCommand request, CancellationToken cancellationToken)
        {
            var dto = new UpdateSampleDto
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive
            };

            await _sampleService.UpdateSampleAsync(request.Id, dto);
            return Unit.Value;
        }
    }
}

namespace OrbitAOS.V6.Application.Features.Samples.Commands
{
    /// <summary>
    /// Command to delete a sample
    /// </summary>
    public class DeleteSampleCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public DeleteSampleCommand(int id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// Handler for DeleteSampleCommand
    /// </summary>
    public class DeleteSampleCommandHandler : IRequestHandler<DeleteSampleCommand, Unit>
    {
        private readonly ISampleService _sampleService;

        public DeleteSampleCommandHandler(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        public async Task<Unit> Handle(DeleteSampleCommand request, CancellationToken cancellationToken)
        {
            await _sampleService.DeleteSampleAsync(request.Id);
            return Unit.Value;
        }
    }
}
