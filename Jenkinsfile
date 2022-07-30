#!groovy

pipeline {
    agent any
    options {
        timestamps()
        disableConcurrentBuilds()
    }

    environment {
        CONTAINER_NAME = "gusmelfordbot"
        CONTAINER_NAME_CONTENT = "gusmelfordbot.contentcollector"
        DOCKER_CONTAINER_TAG = "latest"
        DOCKER_REPO = "gsmelford"
        PORT = "5665"
    }

    stages {
        stage("Update and preparation docker-compose") {
            steps {
                echo "=== stop the old docker-compose ==="
                sh "docker-compose down"
                echo "=== docker pull ==="
                sh "docker-compose pull"
            }
        }
        stage("Up docker-compose") {
            steps {
                echo "=== running docker-compose ==="
                sh "docker-compose up -d"
                echo "=== docker prune ==="
                sh "docker system prune -a -f"
            }
        }
    }
    post {
        always {
            sh "docker logout"
        }
    }
}