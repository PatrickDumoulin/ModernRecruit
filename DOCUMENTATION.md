# ModernRecrut

## Description
ModernRecrut est une solution de recrutement conçue pour améliorer et automatiser le processus de recrutement des employés à travers une architecture de microservices moderne et agile.

## Architecture
Le système est structuré autour de microservices indépendants, chacun responsable d'un aspect spécifique du processus de recrutement :
- **Gestion des offres d'emploi** : Gère la création, la modification, l'affichage et la suppression des offres d'emploi.
- **Gestion des favoris** : Permet aux utilisateurs de marquer des offres d'emploi comme favorites et de les gérer.
- **Gestion des utilisateurs** : Contrôle les comptes utilisateurs et les rôles, ainsi que l'authentification et les autorisations.
- **Gestion des postulations** : Gère les postulations des candidats et permet aux RH d'ajouter des notes et des commentaires.
- **Gestion des documents** : Permet aux candidats de téléverser et gérer leurs documents personnels tels que CV et lettres de motivation.

L'application inclut également une interface web ASP.NET Core MVC et une application mobile Android pour interagir avec les microservices.

## Fonctionnalités Implémentées
Le projet implémente une variété de fonctionnalités clés :
- **CRUD pour les offres d'emploi** : Les utilisateurs peuvent créer, lire, mettre à jour et supprimer des offres d'emploi via une interface web.
- **Gestion des favoris** : Les utilisateurs peuvent ajouter des offres d'emploi à leurs favoris et les visualiser pendant une durée limitée.
- **Gestion des utilisateurs** : Les utilisateurs peuvent s'inscrire, se connecter et gérer leur profil. Les administrateurs peuvent créer et assigner des rôles.
- **Gestion des postulations** : Les candidats authentifiés peuvent postuler à des offres d'emploi, et les RH peuvent consulter et évaluer ces postulations.
- **Gestion des documents** : Téléversement, affichage et suppression de documents par les utilisateurs authentifiés.
- **Sécurité et authentification** : Implémentation d'un système d'authentification pour sécuriser l'accès aux fonctionnalités critiques.
- **Validations diverses** : Vérification de la présence de CV et lettre de motivation pour la postulation, ainsi que des validations sur les dates et les prétentions salariales.

## Tests Unitaires
Des tests unitaires ont été développés pour assurer la qualité et la fiabilité de l'application, en se concentrant particulièrement sur le service de génération des évaluations et les méthodes HTTPPost des contrôleurs MVC. Les tests utilisent xUnit, Moq, Autofixture et FluentAssertions pour couvrir les fonctionnalités de manière exhaustive.

## Prérequis
- .NET Core SDK
- Visual Studio ou un IDE compatible
- SQLite pour la gestion des données

## Installation et Configuration
1. Clonez le dépôt sur votre machine locale.
2. Ouvrez la solution dans Visual Studio.
3. Restaurez les packages NuGet.
4. Construisez la solution pour vérifier qu'il n'y a pas d'erreurs.
5. Exécutez les projets de microservices tous ensemble.
6. Démarrez l'application web MVC pour interagir avec les services backend.

## Contact
Si vous avez des questions ou des suggestions concernant ce projet, n'hésitez pas à contacter Patrick Dumoulin à l'adresse dumoulinpatrick357@gmail.com

