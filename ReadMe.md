# Pizza Pizza Pizza API Implementation
An implementation of the API outlined in the PDF in this repo.

## Some things to note
- This API has passed "Happy Path" testing. Some data validation is present, but it is not comprehensive. Therefore the ugly errors associated with brittle software can be expected when deviating from the happy path.
- JWTs are implemented without a refresh token, so login is required again after 2 hours
- The program sees its non-expired JWTs as valid. So you will stay logged in if you restart the program, and possess a valid token.
- When a JWT is issued it is saved as a cookie to allow Swagger to access restricted pages. But Bearer Token Auth headers still work, meaning Postman can be used to test the API if that is preferred.
- Logout has not been implemented, but logging in as a different user will overwrite your token, letting you assume the new user's identity
- Due to my lack of familiarity with Entity Framework, automatic key generation was proving troublesome. I opted to use Linq statements to calculate the Ids, but I recognize that is not an ideal decision. 
- Regarding data models, the ones used here are adequate, but I'd prefer to retool them a bit given the chance.
- The JWT Key is stored in appsettings, I feel it necessary to state that this is a bad security practice.

## Happy Path Testing
Given the admittedly brittle nature of the software, I felt it may be helpful to document steps for feature verification.
Running the solution in debug mode will launch swagger, if that is preferred. But postman can also be used.

1. Attempt to place an order with dummy data, and recieve a 403 error. 
2. Create an admin user with the User endpoint, defining the role as "admin"
3. Create a regular user with the User endpoint, definging the role as "user"
4. Login as admin with the Login endpoint.
5. Add two pizzas to the menu using the Admin/pizza endpoint
6. Test price modification, test pizza deletion
7. Login as a regular user
8. Attempt to use the admin endpoint, recieve a 403
9. Place an order with valid data
10. Browse the menu/detail endpoints.
11. Use an incognito/private tab, delete the cookie named "AccessToken", or remove the authorization header, and confirm the menu/menu detail requests are still working.


## API Endpoint behavior
### /api/Admin/pizza
This is designed to add/remove pizzas to/from the menu, and update pizza price and descriptions.
Unauthorized users can see this endpoint, but will recieve a 403 forbidden error on interaction.
Authorization is derived from a user with a Role of "admin" - case sensitive, which results in a JWT with a ["Role":"admin"] claim.

Full Schema
```
{
id	integer($int64)
name	string nullable
price	number($double) nullable
description	string nullable
isDelete	boolean nullable
isUpdate	boolean nullable
}
```
#### POST - Add Pizza
Required Schema
```
{
  "name": "string",
  "price": 0,
  "description": "string",
  "isDelete": false,
  "isUpdate": false
}
```

Id is calculated server side, and can be removed from the payload if desired.
Name, description, and price can be omitted, but that may cause errors with the menu and ordering.
isDelete, and isUpdate flags must be set to false to add the pizza to the menu.

#### POST - Remove Pizza
Required Schema
```
{
  "id":5,
  "isDelete": true,
  "isUpdate": false
}
```

Only Id, and the isDelete/isUpdate flags are required, the other fields can be included without causing an issue
isDelete must be set to true, id must be a valid id, however an invalid ID is gracefully handled. 

#### POST - Update Pizza Price
Required Schema
```
{
  "id":3,
  "price": 10,
  "isDelete": false,
  "isUpdate": true
}
```
id, price, and the isDelete/isUpdate flags are required as shown. 
name, and description can be included, and will modify the record as well

#### PUT - Update Pizza Record
Required Schema
```
{
  "id":3,
  "price": 10,
  "isDelete": false,
  "isUpdate": true
}
```
Required Path : /api/Admin/pizza/3

This endpoint isn't explicitly requested by the design doc, but I wanted to include it in case there was an implicit expectation of following REST standards.
The schema and behaviour remains the same as in post-update, but the Id supplied in the path must match the Id supplied in the payload.

#### DELETE - Remove
No Schema needed, only the path is used.
Required Path : api/admin/pizza/{id}
The record whose id matches {id} will be deleted. Once again, supplying Ids that do not exist will result in a graceful failure. 

### /api/Login
This endpoint allows users to authenticate/authorize.
This endpoint handles password authentication/verification and creates JWT tokens on successful logins.
#### Post
Required Schema
  ```
{
    "name": "user",
    "password": "guest"
}
```

Successful login returns a JWT token in the authorization header, adds a token cookie for Swagger compatibility, and outputs the token to the response body for convenience.

### /api/Menu
#### Get
Path : /api/Menu
This Endpoint outputs a JSON array of menu items.
Example : 
```
  [
    {
      "id": 1,
      "name": "cheese",
      "price": 10
    },
    {
      "id": 2,
      "name": "pepperoni",
      "price": 15
    },
    {
      "id": 3,
      "name": "supreme",
      "price": 20
    },
    {
      "id": 4,
      "name": "hawaiian",
      "price": 15
    }
  ]
```

#### Get Detail
Path : /api/Menu/{id}
This endpoint provides a detail view of a menu item
Example Path : /api/Menu/1
Example Output: 
```
  {
    "id": 1,
    "name": "cheese",
    "price": 10,
    "description": "a cheese pizza."
  }
```

### /api/Order
#### Post
Required Schema
```
{
  "customerName": "string",
  "pizzaItems": [
    0
  ],
  "orderTip": 0,
  "isDelivery": false/true,
  "isPickup": true/false,
  "deliveryAddress": "string", 
}
```

Path: /api/Order
This endpoint allows a user to place an order. 
It is restricted unless they are logged in. 
The pizzaItems array must contain valid id's, the customer name must be valid.
If isDelivery is true, isPickup must be false, and vice versa
Delivery Address must be included if isDelivery is true, it can be omitted if isPickup is true.
Delivery Address ommission fails gracefully
An invalid name fails gracefully
Ordering a pizza that does not exist fails badly. 

**Deviation from design doc** While there was no explicit request to include a field for a tip, or calculate a bill total, it seemed pertinent to include these fields and behaviour.

Example Output: 
```
{
  "customerName": "admin",
  "orderNumber": 2,
  "orderTimeStamp": "2024-03-18T06:18:13.8890338Z",
  "pizzaItems": [
    1
  ],
  "orderTotal": 12,
  "orderTip": null,
  "isDelivery": false,
  "isPickup": null,
  "deliveryAddress": null,
  "isComplete": null
}
```

### api/User
#### Post
Required Schema
```
{
  "name": "steve",
  "credential": "guest",
  "role": "scuba"
}
```
This endpoint functions as a signup form. 
Users are allowed to declare their own roles, purely for testing convenience. 
A role must be included, but only the role of "admin" will provide special priveleges.
The credential field becomes the user's password.
If an Id property is included, it gets discarded by the controller. 

## Handlers
Handlers contain validation, and data calculation logic. They are held in the Services folder. Order, PizzaItem, and User handlers all deal with their related entities and handle data validation and calculations, as well as executing the database calls.
### AuthHandler
The Auth Handler takes care of password hash/salting, password verification, and JWT creation.



