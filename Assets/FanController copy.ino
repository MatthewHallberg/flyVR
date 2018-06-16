#include <ESP8266WiFi.h>
#include <WiFiUdp.h>

int pin5 = 14;
int pin6 = 12;
int pin7 = 13;

int incomingByte = 0;

//wifi stuff
const char* ssid     = "WillTradePasswordForBeer"; // wifi network name
const char* password = "02civicsi"; // wifi network password

WiFiUDP Udp;
unsigned int localUdpPort = 1999;
char incomingPacket[255];

void setup(void) {
  Serial.begin(115200);
  delay(10);
  
// We start by connecting to a WiFi network
  Serial.print("Connecting to ");
  Serial.println(ssid);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
      delay(500);
      Serial.print(".");
  }
  Serial.println("WiFi connected"); 
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
  Serial.println("Starting UDP");
  Udp.begin(localUdpPort);  

//set pin modes
  pinMode(pin5, OUTPUT);//0
  pinMode(pin6, OUTPUT);//1
  pinMode(pin7, OUTPUT);//2
  delay(100);
  //set default state
  low();
}

void off(){
  Serial.println("OFF");
  digitalWrite(pin5,HIGH);
  digitalWrite(pin6,HIGH);
  digitalWrite(pin7,HIGH);
}

void low(){
  Serial.println("LOW");
  digitalWrite(pin5,HIGH);
  digitalWrite(pin6,LOW);
  digitalWrite(pin7,HIGH);
}

void medium(){
  Serial.println("MEDIUM");
  digitalWrite(pin5,HIGH);
  digitalWrite(pin6,HIGH);
  digitalWrite(pin7,LOW);
}

void high(){
  Serial.println("HIGH");
  digitalWrite(pin5,LOW);
  digitalWrite(pin6,HIGH);
  digitalWrite(pin7,HIGH);
}

void loop() {

  //listen for packets
  int packetSize = Udp.parsePacket();
  if (packetSize){
    int len = Udp.read(incomingPacket, 255);
    if (len > 0){
      incomingPacket[len] = 0;
    }
    Serial.printf("UDP packet contents: %s\n", incomingPacket);
    if (incomingPacket[4] == 'H'){
        high();
    } else if (incomingPacket[4] == 'L'){
        low();
    }
  }

  if (Serial.available() > 0) {
    // read the incoming byte:
    incomingByte = Serial.read();
    //set fan based on input
    switch (incomingByte) {
      case '1':
        off();
        break;
      case '2':
        low();
        break;
      case '3':
        medium();
        break;
      case '4':
        high();
        break;
      default:
        off();
    }
  }
}
