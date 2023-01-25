# tcp_infinity_snake
A simple snake game written in C#.
The main purpose of this project is to train using the TCP protocol,
and to train synchronization between multiple machines.

## Network overview
One machine is the "leader", and the other machine is the "follower".
The leader is the source of truth and the client, so in case of conflict
the leader's state is the one that is used.

