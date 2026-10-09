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
                bat 'dotnet restore MonProjetCS.sln'
                bat 'dotnet build MonProjetCS.sln --configuration Release'
            }
        }

        stage('Build C++') {
            steps {
                // Exemple avec CMake sous Windows (ou sh/bash sous Linux)
                dir('cpp-folder') {
                    bat 'cmake -B build -S . -DCMAKE_BUILD_TYPE=Release'
                    bat 'cmake --build build --config Release'
                }
            }
        }

        stage('Tests') {
            steps {
                // Exécution des tests unitaires
                bat 'dotnet test MonProjetCS.sln'
                // bat 'build/tests/unit_tests' // Pour le C++
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
