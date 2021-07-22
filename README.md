# SoccerMatchVisualization
 
An example application for showing a replay of a soccer match, including pass viability.

Things left to do due to not being my expertise:
- Fancy visuals (animations/models etc)
Things left to do due to not being big enough in scope or not feasable within current time constraints:
- Adding an inbetween layer of abstraction for the events on the UI
and the replay player inbetween using something like https://github.com/unity-atoms/unity-atoms
- Splitting up the UI in a separate scene for an improved workflow in a team setting
(Less likely to create merge conflicts)
- Unit testing for calculating passing-safety (unsure of the exact criteria just yet)
- A process to filter out faulty data (some players being in an unknown team)
