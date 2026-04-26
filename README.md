# Node Churn Analysis

## Overview

This repository contains simulation tools for analyzing **message propagation under node churn** in network topologies. The primary objective is to quantify how topology influences reliability and delay when nodes randomly transition between live and failed states.

The current implementation focuses on **path** and **cycle** graph families and evaluates their behavior under stochastic churn conditions.

---

## Repository Structure

```
Node-Churn-Analysis/
├── README.md
├── NetworkSimulation/
│   ├── Program.cs
│   ├── TrialRunner.cs
│   ├── Message.cs
│   ├── PathExperiment.cs
│   ├── CycleExperiment.cs
│   ├── ResultAggregator.cs
│   ├── TopologyComparisonReporter.cs
│   └── ...
```

* **NetworkSimulation/**
  C# simulation framework for modeling message propagation and delay under node churn.

---

## Core Concepts

* **Node Churn**
  Each node independently transitions between live and failed states according to a probabilistic model.

* **Message Delay**
  The time required for a message to propagate from a source node to a target node.

* **Topology Effects**
  Structural differences (e.g., redundancy in cycles) influence both:

  * Probability of zero-delay delivery
  * Average message delay

---

## Experimental Design

Two topology families are evaluated:

* **Path Graphs (n nodes)**
  Linear topology with a single path between endpoints.

* **Cycle Graphs (2n nodes)**
  Ring topology with two disjoint paths between source and target.

### Comparison Methodology

To ensure a fair comparison, experiments align **source-to-target distance**, not graph size:

```
Path graph with n nodes  ↔  Cycle graph with 2n nodes
```

This ensures both topologies represent equivalent propagation distance while differing in redundancy.

---

## Metrics

For each experiment, the following metrics are computed:

* **ZeroDelayPercent**
  Percentage of trials with immediate delivery

* **AllNodesLivePercent**
  Percentage of trials where all nodes are operational

* **ReliabilityGainPercent**
  Difference between ZeroDelayPercent and AllNodesLivePercent

* **Average Message Delay**

* **Average Live Node Percentage**

---

## Cross-Topology Comparison

A post-processing step compares results from path and cycle experiments:

* **Delay Reduction**

  ```
  PathDelay − CycleDelay
  ```

* **Delay Reduction Percentage**

  ```
  ((PathDelay − CycleDelay) / PathDelay) × 100
  ```

### Observed Results (Sample)

```
path n=5  vs cycle n=10: reduction ≈ 35%
path n=10 vs cycle n=20: reduction ≈ 24%
path n=15 vs cycle n=30: reduction ≈ 26%
```

**Key Insight:**
Cycle topologies reduce average message delay by approximately **24%–35%** relative to path topologies when controlling for propagation distance.

---

## Running the Simulation

Execution is controlled via a variable in `Program.cs`:

```csharp
string runMode = "all";
// Options: "path", "cycle", "compare", "all"
```

### Modes

* `"path"` → Run path experiments only
* `"cycle"` → Run cycle experiments only
* `"compare"` → Compare previously generated outputs
* `"all"` → Run all steps sequentially

---

## Output Files

Experiments produce text files including:

* `avg_msg_delays_path.txt`
* `avg_msg_delays_cycle.txt`
* `delay_reduction_path_vs_cycle.txt`
* `delay_reduction_percent_path_vs_cycle.txt`

These files are used for post-processing and analysis.

---

## Future Work

* Extend to additional topologies (mesh, random, small-world)
* Formalize probabilistic models of delay distributions
* Derive analytical approximations for redundancy-driven delay reduction
* Integrate visualization and statistical validation tools

---

## Summary

This project demonstrates that **topological redundancy materially reduces message delay under node churn**, even when controlling for propagation distance. The framework is designed to support further experimental and analytical exploration of distributed systems under failure conditions.
