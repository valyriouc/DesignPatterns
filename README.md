# DesignPatterns

# Creational patterns
## Factory methods 
Factory Method is a creational design pattern that provides an interface for creating objects in a superclass, but allows subclasses to alter the type of objects that will be created. - refactoring.guru

## Builder 
Builder is a creational design pattern that lets you construct complex objects step by step. The pattern allows you to produce different types and representations of an object using the same construction code. - refactoring.guru

## Prototype 
Prototype is a creational design pattern that lets you copy existing objects without making your code dependent on their classes. - refactoring.guru
- .NET supports this pattern by design through an interface called I
# Structural patterns 
## Adapter 
Adapter is a structural design pattern that allows objects with incompatible interfaces to collaborate. - refactoring.guru

## Decorator
Decorator is a structural design pattern that lets you attach new behaviors to objects by placing these objects inside special wrapper objects that contain the behaviors. - refactoring.guru

## Proxy
Proxy is a structural design pattern that lets you provide a substitute or placeholder for another object. A proxy controls access to the original object, allowing you to perform something either before or after the request gets through to the original object. - refactoring.guru

# Behavioral patterns
## Chain of responsibility 
Chain of Responsibility is a behavioral design pattern that lets you pass requests along a chain of handlers. Upon receiving a request, each handler decides either to process the request or to pass it to the next handler in the chain. - refactoring.guru

### Examples:
- ASP.NET middleware concept 


## Strategy
Strategy is a behavioral design pattern that lets you define a family of algorithms, put each of them into a separate class, and make their objects interchangeable. - refactoring.guru

## Observer 
Observer is a behavioral design pattern that lets you define a subscription mechanism to notify multiple objects about any events that happen to the object they’re observing.

### Examples:
- MQTT is designed around this pattern 

[] {Name} {IsAppointment:true(Datetime)/false} {FinishDate:Datetime}