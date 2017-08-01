# Checkout.Cart

#### Shopping Cart Rest API



#### Usage examples

##### Create a new basket
```
POST /api/cart/new HTTP/1.1
Host: localhost:5000
Authorization: sk_test_32b9cb39-1cd6-4f86-b750-7069a133667d
Cache-Control: no-cache
Postman-Token: a643daca-539e-7434-2265-76e962fc2403
Content-Type: multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW
```

##### Get full content of basket

```
GET /api/cart/035c3a6b-cbbb-44c3-9394-2cf374120c32 HTTP/1.1
Host: localhost:5000
Authorization: sk_test_32b9cb39-1cd6-4f86-b750-7069a133667d
Cache-Control: no-cache
Postman-Token: 9a5c9d2a-1bb8-49d5-c994-432973402c7b
```

##### Add item to basket

```
POST /api/cart/035c3a6b-cbbb-44c3-9394-2cf374120c32 HTTP/1.1
Host: localhost:5000
Authorization: sk_test_32b9cb39-1cd6-4f86-b750-7069a133667d
Content-Type: application/json
Cache-Control: no-cache
Postman-Token: b6c65cf8-7a8b-ee5c-9fea-cad1680c2efc

{ Name: "Sprite", Quantity: 3}
```

##### Update basket item

```
PUT /api/cart/035c3a6b-cbbb-44c3-9394-2cf374120c32 HTTP/1.1
Host: localhost:5000
Authorization: sk_test_32b9cb39-1cd6-4f86-b750-7069a133667d
Content-Type: application/json
Cache-Control: no-cache
Postman-Token: bd0da7cf-ac09-fdd3-12fb-ec6ea3109e2c

{ Name: "Sprite", Quantity: 1}
```

##### Get single basket item

```
GET /api/cart/035c3a6b-cbbb-44c3-9394-2cf374120c32/Sprite HTTP/1.1
Host: localhost:5000
Authorization: sk_test_32b9cb39-1cd6-4f86-b750-7069a133667d
Content-Type: application/json
Cache-Control: no-cache
Postman-Token: 9808c713-6a07-22d6-ef94-290a5f526eba
```

##### Delete basket item

```
DELETE /api/cart/035c3a6b-cbbb-44c3-9394-2cf374120c32/Sprite HTTP/1.1
Host: localhost:5000
Authorization: sk_test_32b9cb39-1cd6-4f86-b750-7069a133667d
Content-Type: application/json
Cache-Control: no-cache
Postman-Token: 900c2e5d-5a4f-e6fc-0201-dac1ae3e6610

```