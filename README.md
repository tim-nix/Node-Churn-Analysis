### Running Experiments

Set runMode in Main():

- "path"    → runs path experiments
- "cycle"   → runs cycle experiments
- "compare" → compares existing output files
- "all"     → runs all steps sequentially

### Comparison Methodology

Path graph with n nodes is compared to cycle graph with 2n nodes
to ensure equivalent source-to-target distance.

Metrics computed:
- Average message delay
- Delay reduction
- Delay reduction percentage