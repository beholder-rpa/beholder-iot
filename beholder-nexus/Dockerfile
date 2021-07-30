FROM rabbitmq:3.8-management

RUN rabbitmq-plugins enable --offline rabbitmq_mqtt rabbitmq_web_mqtt

# RabbitMQ Management Port
EXPOSE 15672

# MQTT Port
EXPOSE 1883

# MQTT Web Port
EXPOSE 15675