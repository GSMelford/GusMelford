#!groovy

pipeline {
    agent any
    options {
        timestamps()
        disableConcurrentBuilds()
    }

    environment {
        CONTAINER_NAME = "gusmelfordbot.core"
        DOCKER_CONTAINER_TAG = "latest"
        DOCKER_REPO = "gsmelford"
        PORT = "5665"
    }

    stages {
        stage("Build docker image") {
            steps {
                script {
                    echo "=== building image ==="
                    sh "docker build -t $DOCKER_REPO/$CONTAINER_NAME:$DOCKER_CONTAINER_TAG ."
                }
            }
        }
        stage("Push docker image") {
            steps {
                echo "=== pushing image ==="
                sh "echo $DOCKERHUB_CREDENTIALS_PSW | docker login -u $DOCKERHUB_CREDENTIALS_USR --password-stdin"
                sh "docker push $DOCKER_REPO/$CONTAINER_NAME:$DOCKER_CONTAINER_TAG"
            }
        }
        stage("Down old docker-compose") {
            steps {
                echo "=== removing old container ==="
                sh "docker-compose down"
            }
        }
        stage("Run docker-compose") {
            steps {
                echo "=== running new image ==="
                sh "docker-compose up -d"
            }
        }
    }
}