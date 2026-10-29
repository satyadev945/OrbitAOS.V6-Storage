using MediatR;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;

namespace OrbitAOS.V6.Application.Features.Samples.Queries
{
    /// <summary>
    /// Query to get all samples
    /// </summary>
    public class GetAllSamplesQuery : IRequest<IEnumerable<SampleDto>>
    {
    }

    /// <summary>
    /// Handler for GetAllSamplesQuery
    /// </summary>
    public class GetAllSamplesQueryHandler : IRequestHandler<GetAllSamplesQuery, IEnumerable<SampleDto>>
    {
        private readonly ISampleService _sampleService;

        public GetAllSamplesQueryHandler(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        public async Task<IEnumerable<SampleDto>> Handle(GetAllSamplesQuery request, CancellationToken cancellationToken)
        {
            return await _sampleService.GetAllSamplesAsync();
        }
    }
}

namespace OrbitAOS.V6.Application.Features.Samples.Queries
{
    using OrbitAOS.V6.Application.Interfaces;

    /// <summary>
    /// Query to get a sample by ID
    /// </summary>
    public class GetSampleByIdQuery : IRequest<SampleDto?>
    {
        public int Id { get; set; }

        public GetSampleByIdQuery(int id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// Handler for GetSampleByIdQuery
    /// </summary>
    public class GetSampleByIdQueryHandler : IRequestHandler<GetSampleByIdQuery, SampleDto?>
    {
        private readonly ISampleService _sampleService;

        public GetSampleByIdQueryHandler(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        public async Task<SampleDto?> Handle(GetSampleByIdQuery request, CancellationToken cancellationToken)
        {
            return await _sampleService.GetSampleByIdAsync(request.Id);
        }
    }
}
