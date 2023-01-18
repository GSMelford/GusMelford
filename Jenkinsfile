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
        USER_INPUT = 'no'
    }

    stages {
        stage("Build") {
            steps {
                echo "====== Building image... ======"
                sh "sudo docker build -t $DOCKER_REPO/$CONTAINER_NAME:$DOCKER_CONTAINER_TAG ."
                echo "====== Build completed ======"
            }
        }
        stage("Build Micro Services") {
            steps {    
                echo "====== Building image... ======"
                sh "sudo docker build -f contentcollector.Dockerfile -t $DOCKER_REPO/$CONTAINER_NAME_CONTENT:$DOCKER_CONTAINER_TAG ."
                echo "====== Build completed ======"
            }
        }
        stage("Deploy") {
            steps {
                echo "====== Pushing image to docker hub... ======"
                sh "echo $DOCKERHUB_CREDENTIALS_PSW | sudo docker login -u $DOCKERHUB_CREDENTIALS_USR --password-stdin"
                sh "sudo docker push $DOCKER_REPO/$CONTAINER_NAME_CONTENT:$DOCKER_CONTAINER_TAG"
                sh "sudo docker push $DOCKER_REPO/$CONTAINER_NAME:$DOCKER_CONTAINER_TAG"
                echo "====== Push completed ======"

                

                echo "====== Pulling docker images... ======"
                sh "sudo docker-compose pull"
                echo "====== Pull completed  ======"

                echo "====== Running docker-compose ======"
                sh "sudo docker-compose up -d"
                echo "====== Docker-compose launched ======"

                echo "======Docker prune ======"
                sh "sudo sudo docker system prune -a -f"
                echo "====== Docker pruned ======" 
            }
        }
    }
    post {
        always {
            sh "sudo docker logout"
        }
    }
}
