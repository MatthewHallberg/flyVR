int pin0 = 16;
int pin1 = 5;
int pin2 = 4;

int incomingByte = 0;

void setup() {
  pinMode(pin0, OUTPUT);//0
  pinMode(pin1, OUTPUT);//1
  pinMode(pin2, OUTPUT);//2
  Serial.begin(115200);
}

void off(){
  Serial.println("OFF");
  digitalWrite(pin0,HIGH);
  digitalWrite(pin1,HIGH);
  digitalWrite(pin2,HIGH);
}

void low(){
  Serial.println("LOW");
  digitalWrite(pin0,HIGH);
  digitalWrite(pin1,LOW);
  digitalWrite(pin2,HIGH);
}

void medium(){
  Serial.println("MEDIUM");
  digitalWrite(pin0,HIGH);
  digitalWrite(pin1,HIGH);
  digitalWrite(pin2,LOW);
}

void high(){
  Serial.println("HIGH");
  digitalWrite(pin0,LOW);
  digitalWrite(pin1,HIGH);
  digitalWrite(pin2,HIGH);
}

void loop() {
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
