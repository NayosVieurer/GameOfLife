pipeline {
    agent any

    stages {
        stage('Récupération du code') {
            steps {
                // Utilise le plugin Git de Jenkins
                checkout scm
            }
        }

        stage('Build C# (.NET)') {
            steps {
                // Exemple pour une solution .NET
                sh 'dotnet restore GameOfLife.sln'
                sh 'dotnet build GameOfLife.sln --configuration Release /p:EnableWindowsTargeting=true'
            }
        }

        stage('Build C++') {
            steps {
                // Exemple avec CMake sous Windows (ou sh/bash sous Linux)
                dir('cpp-folder') {
                    sh 'cmake -B build -S . -DCMAKE_BUILD_TYPE=Release'
                    sh 'cmake --build build --config Release'
                }
            }
        }

        stage('Tests') {
            steps {
                // Exécution des tests unitaires
                sh 'dotnet test GameOfLife.sln'
                // sh 'build/tests/unit_tests' // Pour le C++
            }
        }
    }

    post {
        always {
            echo 'Nettoyage ou archivage des artefacts si nécessaire.'
        }
        failure {
            echo 'Le build a échoué !'
        }
    }
}
