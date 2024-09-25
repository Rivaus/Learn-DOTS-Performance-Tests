# LEARN DOTS - Performance Tests

As a beginner with Unity  Data-Oriented Technology Stack (DOTS), I wanted to do some performance tests to compare MonoBehaviours and Entites.

![image info](game-screenshot.png);

### Situation
The scene has 10 000 objects which are seekers. Seekers look for the closest target and move toward it. There are 20 targets per scene.
They look for the closest target each frame because targets can by moved at any time when a user presses space key.
I didn't implement optimizations (such as only updating the closest target when targets are moved) because most optimizations can be implemented either with MonoBehaviours or Entities.

### Solution 1 : Individual MonoBehaviour
I started with the worst implementation : each seeker as a ```TargetSeeker``` script (```MonoBehavior```).

### Solution 2 : Manager MonoBehaviour
This solution keeps using traditional monobehaviours but only one : a global manager which iterates over seekers and targets.

### Solution 3 : Jobified Manager MonoBehaviour
My global manager iterates now on seekers on multi threads thanks to [```IJobParallelForTransform```](https://docs.unity3d.com/ScriptReference/Jobs.IJobParallelForTransform.html). I also added Burst Compiler for this solution

### Solution 4 : Simple ECS
I totally switched from MonoBehaviours to Entities for this example.

### Solution 5 : Simple Burst ECS
Same as solution 4 but with Burst Compiler.

### Solution 6 : ECS + Job
Now my Move Seeker System uses jobs to multithread computations.

### Solution 7 : ECS + Job + Burts (Full DOTS stack)
Same as solution 6 but with Burst Compiler.

### Test conditions

Computer :
- AMD Ryzen 7 7700, AMD RX 7800 XT, 32 GO DDR5 (6000MHz), MSI B650 Gaming Plus Wifi

Resolution : 1255px x 613 px

Unity and package versions :
- Unity 6 (6000.0.10f1)
- Burst 1.8.16
- Collections 2.4.2
- Entities 1.2.3
- Entities Graphics 1.2.3
- URP 17.0.3


### Results

Average are computed from several random frames picked from the profiler (numbers are different enough no to be very accurate).

|                    |  Moving Seeker Time (ms) | Player Loop (ms) | Gain over solution 2 |
| :---               |    :----:                |    :----:        |    :----:            |
| Solution 1         |  19.07                   | 19.94            | -82%                 |
| Solution 2         |  10.08                   | 10.94            | 0                    |
| Solution 3         |  0.64                    | 1.82             | 83%                  |
| Solution 3 (Burst) |  --                      | 1.37             | 87%                  |
| Solution 4         |  8.02                    | 7.52             | 31%                  |
| Solution 5         |  0.97                    | 1.40             | 87%                  |
| Solution 6         |  0.53                    | 0.94             | 91%                  |
| Solution 7         |  0.06                    | 0.6              | 95%                  |