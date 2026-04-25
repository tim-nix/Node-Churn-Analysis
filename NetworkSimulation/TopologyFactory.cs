namespace NetworkSimulation
{
    public static class TopologyFactory
    {
        public static Network CreatePath(int numNodes)
        {
            return new Network(CommonGraphs.Path(numNodes));
        }
    }
}
