# TODO / Roadmap

## Testovi
- [ ] Izvući BFS/DFS/Dijkstra/A* iz MazeSolver stage-mašine u zasebnu
      `Pathfinder` klasu koja prima Maze i vraća put (čiste funkcije).
      MonoBehaviour onda samo vodi vizuelizaciju.
- [ ] EditMode unit testovi za PriorityQueue i Maze (setup sa Game.asmdef
      + Tests/EditMode). Posle izvlačenja Pathfinder-a, dodati i testove
      koji proveravaju da BFS/Dijkstra/A* nalaze put iste dužine na
      lavirintu bez težina.

## Bugovi
- [ ] Reset vizuelizacije kad se algoritam promeni tokom pauze: ako se
      pauzira na pola, promeni algoritam i klikne Play, ništa se ne dešava
      (stage != 0, pa novi algoritam ne prođe inicijalizaciju). Detektovati
      promenu algoritma i forsirati reset pre nastavka.

## Manje izmene / čišćenje
- [ ] Jedan izvor za string "Final time" (const u MazeSolver-u) da
      SaveTime i ClearFinalTime ne mogu da se raziđu (kao case-sensitivity bug).
- [ ] Preispitati da li Dijkstra treba da posećuje već viđene susede
      (sad koristi skipVisited=true, A* koristi false) radi korektne relaksacije.