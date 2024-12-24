# Kafka Streams Data Processing Project

This project demonstrates how to process and transform data using Kafka Streams in .NET. It provides a simple, minimal approach to implement real-time data streaming and processing within a Kafka ecosystem + Streamiz.Kafka.Net in .NET . The goal is to build a scalable, stateful, and fault-tolerant system capable of handling high-throughput data streams, processing events in real time.

### Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [How to Run](#how-to-run)
- [Processing Data](#processing-data)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

This project focuses on processing events produced by Kafka producers, transforming and filtering data using Kafka Streams. The core of the implementation includes:

- **Stateful Processing**: Ensuring that the stream processing is aware of the current state of data.
- **Exactly-Once Semantics**: Guaranteeing that each event is processed exactly once, even in the case of failures.
- **Scalability**: The solution can scale horizontally to handle more traffic as the workload increases.

Kafka Streams provide a powerful and easy-to-use library for building stream processing applications. However, some libraries, such as **Apache Flink**, **Apache Beam**, and **Apache Spark**, can offer more advanced features and flexibility, especially for distributed processing at scale.

---

## Prerequisites

To run this project, you need the following installed:

- **Docker**: To run Kafka and other necessary services in containers.
- **.NET 6 SDK or later**: For building and running the project.
- **Kafka (via Docker)**: Kafka and Zookeeper to run locally in containers.
- **Producer and Consumer Setup**: You will also need Kafka producers and consumers running to produce and consume data.

### Required Docker Services

- Kafka
- Zookeeper

---

## How to Run

1. **Start Kafka and Zookeeper using Docker:**

   You can start Kafka and Zookeeper using Docker Compose or using individual Docker commands. Below is an example `docker-compose.yml`:

2. **Start Projects !**
