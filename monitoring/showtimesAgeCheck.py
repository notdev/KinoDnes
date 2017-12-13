from checks import AgentCheck
import urllib2

class ShowtimesAgeCheck(AgentCheck):
    def check(self, instance):
        response = urllib2.urlopen('https://api.kinodnes.cz/api/kino/get/showtimesage')
        response_content = response.read()
        age = int(response_content)
        self.gauge('kinodnesapi.showtimesage', age)
        
if __name__ == '__main__':
    check, instances = ShowtimesAgeCheck.from_yaml('/etc/dd-agent/conf.d/showtimesAgeCheck.yaml')
    for instance in instances:
        print "\nRunning the check"
        check.check(instance)
        print "\nCheck finished"
        