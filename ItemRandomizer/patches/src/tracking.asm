arch snes.cpu
lorom

// Addresses to helper functions for stat tracking
define inc_stat $dfd800
define dec_stat $dfd840
define store_stat $dfd880
define load_stat $dfd8b0

// RTA Timer (timer 1 is frames, and timer 2 is number of times frames rolled over)
define timer1 $05b8
define timer2 $05ba





