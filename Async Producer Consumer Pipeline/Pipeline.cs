using System;


namespace ErikTheCoder.Sandbox.AsyncPipeline
{
    public static class Pipeline
    {
        public static IPipeline Create(int Version)
        {
            return Version switch
            {
                1 => new PipelineV1(),
                2 => new PipelineV2(),
                3 => new PipelineV3(),
                _ => throw new NotSupportedException($"Pipeline version {Version} not supported.")
            };
        }
    }
}
