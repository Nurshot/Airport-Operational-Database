from kafka import KafkaProducer
import json

producer = KafkaProducer(
    bootstrap_servers="localhost:29092",
    value_serializer=lambda v: json.dumps(v).encode("utf-8")
)

producer.send("aodb-logs", {"level": "INFO", "message": "Test logu", "timestamp": "2025-09-15T18:35:00Z"})
producer.flush()
