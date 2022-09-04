Feature: Rooms

Background:
 * url baseUrl+"/api/Rooms"
 * def expectedRoomsList = read('../data/rooms.json')
 * def uuid = function(){ return java.util.UUID.randomUUID() + '' }
 * def id = 'Karate-' +uuid() 

Scenario: Fetch all rooms
 When method GET
 Then status 200
 And match $ contains expectedRoomsList 

Scenario: Create room
    Given request { "numberOfSeats": 15, "hasProjector": true, "hasSoundSystem": true, "hasAirConditioner": true, "name": '#(id)' }
    When method POST
    Then status 201
    And match header Location == baseUrl + "/api/Rooms/" + id

    * def location = responseHeaders['Location'][0]

    Given url location
    When method get
    Then status 200
    And match response == { "numberOfSeats": 15, "hasProjector": true, "hasSoundSystem": true, "hasAirConditioner": true, "name": '#(id)' }