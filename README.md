# Komunikátor s využitím UDP protokolu

PKS project for school - own chat communicator

Nad protokolom UDP (User Datagram Protocol) transportnej vrstvy sieťového modelu 
TCP/IP navrhnite a implementujte program, ktorý umožní komunikáciu dvoch účastníkov 
v sieti Ethernet, teda prenos správ ľubovoľnej dĺžky medzi počítačmi (uzlami). 
Program bude pozostávať z dvoch častí – vysielacej a prijímacej. Vysielací uzol pošle 
správu inému uzlu v sieti. Predpokladá sa, že v sieti nedochádza k stratám dát. Vysielajúca 
strana rozloží správu na menšie časti - fragmenty, ktoré samostatne pošle. Správa sa 
fragmentuje iba v prípade, ak je dlhšia ako max. veľkosť fragmentu. Veľkosť fragmentu musí 
mať používateľ možnosť nastaviť. 
Po prijatí správy na cieľovom uzle tento správu zobrazí. Ak je správa poslaná ako 
postupnosť fragmentov, najprv tieto fragmenty spojí a zobrazí pôvodnú správu. 

Program musí mať nasledovné vlastnosti: 

1. Pri posielaní správy musí používateľovi umožniť určiť cieľovú stanicu. 
2. Používateľ musí mať možnosť zvoliť si max. veľkosť fragmentu. 
3. Obe komunikujúce strany musia byť schopné zobrazovať: 
  - poslanú resp. prijatú správu, 
  - počet fragmentov správy. 

Program musí byť organizovaný tak, aby oba komunikujúce uzly mohli byť (nie 
súčasne) vysielačom a prijímačom správ. Táto verzia je schopná full duplex-u.
