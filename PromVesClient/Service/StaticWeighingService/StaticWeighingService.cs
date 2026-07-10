using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Service.StaticWeighingService
{
    public class StaticWeighingService
    {
        private readonly ILogger<StaticWeighingService> _logger;

        private readonly ApplicationDbContext _dbContext;
        //private double Platform1Left { get; set; }
        //private double Platform1Right { get; set; }
        //private double Platform2Left { get; set; }
        //private double Platform2Right { get; set; }
        public StaticWeighingService(ILogger<StaticWeighingService> logger, ApplicationDbContext dbContext)
        { 
            _logger = logger;
            _dbContext = dbContext;
        }
        //метод отвечающий за сохранение данных взвешивания
        public async Task<ServiceResult> saveWeighingAsync(double Platform1Left, double Platform1Right, double Platform2Left, double Platform2Right, string VagonNumber, string TypeWeighing)
        {
            double WeightSum = Platform1Left + Platform1Right + Platform2Left + Platform2Right;


            return ServiceResult.Ok();
        }
    }
}
