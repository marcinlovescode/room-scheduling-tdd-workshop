# Room Scheduling using Test Driven Development (TDD) approach

## The purpose of this repo 
This repo meant is to explain by example how you can use Test Driven Development approach to implement Web API from scratch according to user stories. 

## How

Commit history shows the whole process of fulfilling requirements. Every single commit is a small step forward. Most of functionalities are implemented in two by two commits. The first one is for the test. The latter makes the test green. In a few places there is a refactor step introduced. 

## Domain

Meeting room management

## User stories

### #1. As an office manager I want to register conference room in the room catalogue so teams can find rooms in case they need a room for a meeting.
\
Acceptance criteria:\
GIVEN @Room details \
WHEN Office manager registers room\
THEN Room is registered with @ Room details 

| Room details              | Type of data              |
| ------------------------- |:------------------------- |
| Number seats of available | Int32                     |
| Room has projector        | Boolean                   |
| Room has sound system     | Boolean                   |
| Room has air conditioner  | Boolean                   | 


</br>

### #2. As a team leader I want have a name assigned to a conference room so I can distinguish rooms.
\
Acceptance criteria:\
GIVEN Conference room\
THEN Room has name assigned

</br>

### #3. As an office manager I want limit a room name to have at least three characters so I can avoid name collisions.
\
Acceptance criteria:\
GIVEN Room details  \
WHEN Office manager registers room \
THEN Room name contains at least three characters

</br>

### #4. As a team leader I want to know which meeting rooms fulfills my needs and when are available so I can decide with my team when do we want to have a meeting
\
Acceptance criteria:\
GIVEN @Requirements and desired date \
WHEN Look for a rooms \
THEN I can find out which fulfils my needs \
AND Which time slots are available 

| Needs                        |
| ---------------------------- |
| Required number of seats     |
| A need for a projector       |
| A need for a sound system    |
| A need for a air conditioner |

</br>

### #5. As a team leader I want book a conference room so I can make sure that it will be available for my room
\
Acceptance criteria:\
GIVEN Room identifier \
AND Date \
AND Time slot \
AND Time slot is free \
WHEN Book room \
THEN Time slot is being booked

| Needs                        |
| ---------------------------- |
| Required number of seats     |
| A need for a projector       |
| A need for a sound system    |
| A need for a air conditioner |

### #6. As a office manager I want to receive a notification with booked time slot on specific date for a room so I can plan a room service
\
Acceptance criteria:\
GIVEN Room identifier \
AND Date \
AND Email address \
WHEN Request the notification with booked time slots \
THEN I receive an email notification containing bookings

## Running locally
0. Install the latest dotnet sdk https://dotnet.microsoft.com/en-us/download
1. Navigate to RoomScheduling.Host: ```cd src/RoomScheduling.Host```
2. Build project: ```dotnet build```
3. Run app: ```dotnet run```

## How make sending emails working
   * Generate your own SendGrid API Key
   * Set API Key by modifying ```sendGridApiKey``` variable in Program.cs file
   * Set your email by modifying ```fromEmail``` variable in Program.cs file 

## How to test
0. Install the latest dotnet sdk https://dotnet.microsoft.com/en-us/download
1. Run tests: ```dotnet test```