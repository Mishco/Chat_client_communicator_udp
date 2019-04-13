# Chat communicator using UDP protocol

Project for school - own chat communicator - Computer and communication networks

## Assignment 

Above the User Datagram Protocol (UDP) of the transport model of the network model
Design and implement a TCP/IP program that allows two participants to communicate
in Ethernet, that is, the transmission of any length of messages between computers (nodes).
The program consist of two parts - broadcasting and receiving. It sends the transmitting node
another network node. It is assumed that there is no loss of data on the network. sending
the party spreads the message to smaller parts - fragments that it sends itself. Manage yourself
fragmented only if it is longer than max. fragment size. The size of the fragment must
have the user the ability to set.
When you receive a message on the destination node, it displays this message. If the message is sent as
fragment sequence, first combines these fragments and displays the original message.

The program must have the following features:

1. When sending a message, it must allow the user to specify the destination.
2. The user must be able to choose max. fragment size.
3. Both communicating parties must be able to display:
  - sent, respectively. received message
  - number of message fragments.

The program must be organized so that the two communicating nodes can be (not
transmitter and receiver. This version is full duplex.

## Slovak version

## Komunikátor s využitím UDP protokolu

PKS project for school - own chat communicator

Nad protokolom UDP (User Datagram Protocol) transportnej vrstvy sieťového modelu 
TCP/IP navrhnite a implementujte program, ktorý umožní komunikáciu dvoch účastníkov 
v sieti Ethernet, teda prenos správ ľubovoľnej dĺžky medzi počítačmi (uzlami). 
Program pozostá z dvoch častí – vysielacej a prijímacej. Vysielací uzol pošle 
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
