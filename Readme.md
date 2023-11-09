OpenAPIASPNET-EventSystem
=======

## About
---

This project implements event storage system

## Infrastructure dependecies
---

* Container environment infrastructure (e.g. podman, Docker or Kubernetes)
* RDBMS PostgreSQL
* RabbitMQ
* (Optional) Graylog
* (Optional, Required for Graylog) MongoDB

## Building image
---

Using docker:
```sh
docker build -t openapiaspnet:dev .
```

Using podman:
```sh
podman build -t openapiaspnet:dev .
```

## Running image
---

Using docker:
```sh
docker run \
 -d --name=webhost \
 -p <HTTP Server port>:80/tcp -p <HTTPS Server port>:443/tcp \
 -e "GRAYLOG_ADDRESS=http://<Host>:9000/graylog" \
 -e "AMQP_HOSTNAME=<Host>" -e "AMQP_PORT=5672" -e "AMQP_USERNAME=<User>" -e "AMQP_PASSWORD=<Password>" \
 -e "POSTGRESQL_CONNECTION=Server=<Host>;Port=5432;Database=<Database>;User ID=<Role>;Password=<Role password>;" \
 openapiaspnet:dev
```

Using podman:
```sh
podman run \
 -d --name=webhost \
 -p <HTTP Server port>:80/tcp -p <HTTPS Server port>:443/tcp \
 -e "GRAYLOG_ADDRESS=http://<Host>:9000/graylog" \
 -e "AMQP_HOSTNAME=<Host>" -e "AMQP_PORT=5672" -e "AMQP_USERNAME=<User>" -e "AMQP_PASSWORD=<Password>" \
 -e "POSTGRESQL_CONNECTION=Server=<Host>;Port=5432;Database=<Database>;User ID=<Role>;Password=<Role password>;" \
 openapiaspnet:dev
```
