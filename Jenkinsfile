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
        stage("Build") {
            steps {
                script {
                    echo "====== Building image... ======"
                    sh "docker build -t $DOCKER_REPO/$CONTAINER_NAME:$DOCKER_CONTAINER_TAG ."
                    sh "docker build -f contentcollector.Dockerfile -t $DOCKER_REPO/$CONTAINER_NAME_CONTENT:$DOCKER_CONTAINER_TAG ."
                    echo "====== Build completed ======"
                }
            }
        }
        stage("Deploy") {
            steps {
                echo "====== Pushing image to docker hub... ======"
                sh "echo $DOCKERHUB_CREDENTIALS_PSW | docker login -u $DOCKERHUB_CREDENTIALS_USR --password-stdin"
                sh "docker push $DOCKER_REPO/$CONTAINER_NAME:$DOCKER_CONTAINER_TAG"
                echo "====== Push completed ======"

                echo "====== Stoping the old docker-compose... ======"
                sh "docker-compose down"
                echo "====== The old docker-compose stoped ======"

                echo "====== Pulling docker images... ======"
                sh "docker-compose pull"
                echo "====== Pull completed  ======"

                echo "====== Running docker-compose ======"
                sh "docker-compose up -d"
                echo "====== Docker-compose launched ======"

                echo "======Docker prune ======"
                sh "docker system prune -a -f"
                echo "====== Docker pruned ======" 
            }
        }
        stage('Micro Services approval'){
            input "Build and deploy Micro Services?"
        }
        stage("Build Micro Services") {
            steps {    
                echo "====== Building image... ======"
                sh "docker build -f contentcollector.Dockerfile -t $DOCKER_REPO/$CONTAINER_NAME_CONTENT:$DOCKER_CONTAINER_TAG ."
                echo "====== Build completed ======"
            }
        }
        stage("Deploy") {
            steps {
                echo "====== Pushing image to docker hub... ======"
                sh "echo $DOCKERHUB_CREDENTIALS_PSW | docker login -u $DOCKERHUB_CREDENTIALS_USR --password-stdin"
                sh "docker push $DOCKER_REPO/$CONTAINER_NAME_CONTENT:$DOCKER_CONTAINER_TAG"
                echo "====== Push completed ======"

                echo "====== Stoping the old docker-compose... ======"
                sh "docker-compose down"
                echo "====== The old docker-compose stoped ======"

                echo "====== Pulling docker images... ======"
                sh "docker-compose pull"
                echo "====== Pull completed  ======"

                echo "====== Running docker-compose ======"
                sh "docker-compose up -d"
                echo "====== Docker-compose launched ======"

                echo "======Docker prune ======"
                sh "docker system prune -a -f"
                echo "====== Docker pruned ======" 
            }
        }
    }
    post {
        always {
            sh "docker logout"
        }
    }
}