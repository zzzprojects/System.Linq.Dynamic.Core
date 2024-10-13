using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;

namespace ConsoleApp_net6._0
{
    public static class Issue837
    {
        public static void Test()
        {
            var selectExp = 
                """
                new {
                		ExpertPoints.Select(
                		new 
                		{
                			Expert.Name,
                			BatchId,
                			ProjectSampleId,
                			ExpertId,
                			Opinion,
                			Result,
                			AttachmentValue,
                			CreateTime,
                			UpdateTime,
                			Deleted,
                			Id
                		}) AS Experts,
                		
                		Project.BatchId AS BatchId,
                		Project AS Project,
                		Id,
                		Result,
                		Opinion,
                		CreateTime,
                		UpdateTime,
                		Deleted
                	}
                """;

            var result = DynamicExpressionParser.ParseLambda<ProjectSample, VProjectSamplePoint>
            (
                new ParsingConfig
                {
                    ResolveTypesBySimpleName = true,
                }, 
                true, 
                selectExp
            );

            Console.WriteLine(result);
        }
    }

    public class Project
    {
        public string BatchId { get; set; } = string.Empty;
    }

    public class User
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public class ProjectSample
    {
        public string Id { get; set; } = string.Empty;
        
        public virtual Project? Project { get; set; }
        
        public int? Result { get; set; }
        
        public string? Opinion { get; set; }

        public virtual List<ProjectSampleExpertPoint>? ExpertPoints { get; set; }
    }

    public class ProjectSampleExpertPoint
    {
        public string Id { get; set; } = string.Empty;

        public string? BatchId { get; set; }
        
        public string? ProjectSampleId { get; set; }

        public virtual ProjectSample? ProjectSample { get; set; }
        
        public string? ExpertId { get; set; }
        
        public virtual User? Expert { get; set; }
        
        public string? Opinion { get; set; }
        
        public int? Result { get; set; }
        
        public string? AttachmentValue { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public bool Deleted { get; set; }
    }

    public class VProjectSample : ProjectSample
    {
        public string? BatchId { get; set; }

        public new Project? Project { get; set; }

        public string? OrganizationName { get; set; }

        public string? OrganizationTel { get; set; }
    }

    public class VProjectSamplePoint : VProjectSample
    {
        public List<VProjectSampleExpertPoint>? Experts { get; set; }

        // public string? ExpertName { get; set; } // Added
    }

    public class VProjectSampleExpertPoint : ProjectSampleExpertPoint
    {
    }
}
