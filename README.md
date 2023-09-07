# Implementation

Implementation includes but is not limited to:

- Accepting Multiple Subreddits
- Implementing API Rate Limiting
- Applying Connection Retry Policy
- Exception Handling
- Logging
- Unit Test Cases
- Pub/Sub Pattern
- Saving Data to Redis Cloud Database

# Architecture

The architecture of the system is as follows:

- Create a Reddit or other social platform service as an independent Service Project.
- Send the output to a Pub/Sub messaging queue (ideally another messaging queue like Redis/RabbitMQ, but for this demonstration, stick to Concurrent Queue).
- Enrich the pub/sub message and store it in the respective data store.
- The analytics client will fetch the data from the data store.

# Software Tools Used

The following software tools were used for this project:

- .Net 7.0
- Rider IDE
- Postman
- Docker Desktop
- Redis (used for demonstrating data store saving)

# Registering Reddit API

To access the Reddit API, you need to register your application:

- Register the app (jhsample) as a script for developer use.
- Use the redirect URI as `http://www.jhsample.com/unused/redirect/uri`.
- Secret: `L1pHKp4uRclvqynV5oOh3ZS7iISLmA`
- Client ID: `pwMQGXbCKWegT87Q2nUfvA`

Preferably, store sensitive information like these in a Secure Key Vault. For now, it's stored in the Reddit Config:

```json
"RedditConfig": {
  "ClientId": "vfwMZ-WFhwmbnlM_358FdQ",
  "ClientSecret": "Od7uUp_lmouw_dodONFKn20YLyQZHg",
  "AuthType": "Basic",
  "TokenUrl": "https://www.reddit.com/api/v1/access_token",
  "GrantType": "client_credentials",
  "UserAgent": "MY USER AGENT"
}

# Redis Cloud Trial Version Key

To access the Redis Cloud trial version, you'll need the following connection details:

- **Host**: `redis-16411.c1.us-east1-2.gce.cloud.redislabs.com`
- **Port**: `16411`
- **Password**: `XTQ2cYN0fsQg0YQcsXwXfOW8Ec6uRyKi`

This key will grant you access to the Redis Cloud trial version for your data storage needs.

# Output - Client UI

![image](https://github.com/phanisivakumar/reddit/assets/37417609/dd2454e0-ddfe-47eb-a110-8158fed1d1ea)



