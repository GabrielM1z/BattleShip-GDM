# BattleShip-GDM

# Niveau des IAs
nous avons fait plusieurs niveau d'IA
- Niveau 0 : L'IA tire systématiquement sur toutes les positions, en parcourant la grille de gauche à droite et de haut en bas, sans aucune logique particulière.

- Niveau 1 : L'IA tire de manière totalement aléatoire sur la grille, sans tenir compte des résultats de ses tirs précédents.

- Niveau 2 : L'IA tire également de manière aléatoire, mais dès qu'elle touche une case, elle passe en "mode chasse" et tire sur toutes les cases adjacentes (haut, bas, gauche, droite) pour essayer de couler le bateau touché. Elle continue de tiré autour même si le bateau est coulé.

- Niveau 3 : Ce niveau reprend le fonctionnement du Niveau 2, mais avec une optimisation supplémentaire lors des tirs aléatoires : l'IA tire une case sur deux, ce qui lui permet de maximiser ses chances de toucher un bateau plus rapidement. Une fois un bateau touché, elle passe en "mode chasse" comme au Niveau 2. Après avoir coulé un bateau, elle repasse en mode recherche aléatoire (cependant, dans certains cas, elle reprend les tires autour d'un bateau coulé).