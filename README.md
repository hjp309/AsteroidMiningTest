# Asteroids Mining Game - Coding Assessment

## Overview
Welcome to the Asteroids Mining Game coding assessment! This is a real-time console-based mining simulation that contains various performance issues, bugs, and architectural problems that you'll need to identify and resolve.

## Your Mission
Your goal is to get this broken codebase running and fix the most obvious performance issues. This assessment is designed for entry-level developers and should take about 2 hours.

## Assessment Phases

### Phase 1: Get It Running (15-20 minutes)
**Goal**: Fix compilation errors and get the program to execute
- The project currently has compilation errors that prevent it from building
- Focus on getting a clean build and basic program execution
- Look for missing using statements, wrong method names, type mismatches

**Success Criteria**:
- Project compiles without errors
- Program starts and runs
- You can see the game initialization messages

### Phase 2: Fix Logic Bugs (25-30 minutes)
**Goal**: Resolve functional issues with game mechanics
- The ship may not move or mine correctly
- Cargo system might not work properly
- Resource collection and delivery may have issues

**Success Criteria**:
- Ship can navigate around the world
- Ship can mine asteroids and collect resources
- Ship can return to base and unload cargo
- Game loop runs stably for several minutes

### Phase 3: Basic Performance Optimization (50-60 minutes)
**Goal**: Find and fix obvious performance problems
- Current implementation has major inefficiencies
- Target: Run with 3000-5000 asteroids at 30+ FPS
- Use the built-in performance monitor to guide your work

**Key Issues to Look For**:
- Nested loops (O(n²) problems)
- Memory leaks (collections growing, event handlers not cleaned up)
- Repeated calculations
- String concatenation in loops
- Blocking operations (Thread.Sleep, forced GC)

**Success Criteria**:
- Game runs smoothly with 3000-5000 asteroids
- Memory usage stays stable (no continuous growth)
- Loop time averages under 33ms (30 FPS)
- No obvious lag or stuttering

### Phase 4: Written Summary (10-15 minutes)
**Goal**: Document what you found and fixed
- Write a brief summary of the problems you identified
- Explain the changes you made and why
- Include before/after performance metrics if you have them

**Format**: Simple text or markdown file covering:
- Compilation errors fixed
- Logic bugs fixed
- Performance issues identified and resolved
- Results achieved

## Getting Started

### Prerequisites
- .NET 8.0 SDK installed
- Any text editor or IDE (Visual Studio, VS Code, Rider, etc.)

### Running the Application
```bash
cd AsteroidsMining
dotnet run
```

### Performance Testing
The game includes built-in performance monitoring that displays:
- Loop execution time
- Memory usage
- Frame rate
- GC pressure indicators

Watch these metrics to validate your optimizations.

### Key Controls
- **ESC**: Exit the game loop
- The game runs automatically - no user input required for normal operation

## What We're Looking For (Entry-Level)

### Technical Skills
- **Debugging**: Can you systematically work through compilation and logic errors?
- **Performance Awareness**: Can you identify obvious inefficiencies (nested loops, memory leaks)?
- **Testing**: Do you validate your changes and check the metrics?
- **Code Reading**: Can you understand existing code and spot problems?

### Problem-Solving Process
- Do you work methodically or make random changes?
- Do you use the performance monitor to guide optimization?
- Do you test your changes before moving on?
- Can you prioritize the biggest problems first?

### Communication Skills
- Can you clearly explain what was broken?
- Can you explain what you changed and why?
- Can you document your work in a simple summary?

## Hints and Tips

### Common Issues to Look For
- **Compilation Errors**: Missing using statements, incorrect method calls, wrong parameters
- **Logic Bugs**: Off-by-one errors, incorrect comparisons, wrong calculations
- **Performance Issues**: O(n²) algorithms, memory leaks, inefficient data structures
- **Architecture Problems**: Tight coupling, violation of SOLID principles, missing abstractions

### Performance Optimization Strategies
- Use profiling to identify actual bottlenecks (don't guess!)
- Consider spatial data structures for collision detection
- Look for opportunities to cache expensive calculations
- Check for memory leaks from event handlers
- Evaluate data structure choices (List vs HashSet vs Dictionary)

### Tools and Techniques
- Built-in performance monitor shows key metrics
- .NET's garbage collection metrics are included
- Use the debugger to step through problematic code sections
- Consider using async/await for I/O operations

## Evaluation Criteria

### Scoring (100 points total)
- **Functionality (25 points)**: Gets program running and fixes logical bugs
- **Performance (35 points)**: Achieves target performance metrics with large scale
- **Code Quality (25 points)**: Applies good architecture and clean code principles  
- **Problem Solving (15 points)**: Demonstrates systematic approach and clear communication

### What Makes a Strong Submission
- **Systematic Approach**: Uses data and profiling to guide optimization decisions
- **Incremental Progress**: Makes focused changes and validates results
- **Clean Implementation**: Maintains or improves code readability while optimizing
- **Clear Communication**: Can explain technical decisions and trade-offs effectively

### Red Flags
- Makes random changes without understanding their impact
- Focuses on micro-optimizations while ignoring major bottlenecks
- Breaks functionality while optimizing performance
- Cannot explain the reasoning behind their changes

## Final Notes

- Don't feel pressured to complete every phase - focus on doing quality work
- It's better to thoroughly complete fewer phases than to rush through all of them
- Ask questions if you need clarification on requirements or expectations
- Remember: we want to see your problem-solving process, not just the final result

Good luck, and happy coding!